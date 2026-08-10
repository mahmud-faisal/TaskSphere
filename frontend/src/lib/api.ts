import { ApiResponse } from '@/types';

const getApiBaseUrl = (): string => {
  const raw = (process.env.NEXT_PUBLIC_API_URL || 'https://localhost:7278/api').trim().replace(/\/$/, '');
  return raw.endsWith('/api') ? raw : `${raw}/api`;
};

// const getApiBaseUrl = (): string => {
//   console.log('ENV:', process.env.NEXT_PUBLIC_API_URL);

//   const raw = process.env.NEXT_PUBLIC_API_URL;

//   if (!raw) {
//     throw new Error('NEXT_PUBLIC_API_URL is not loaded');
//   }

//   return raw.trim().replace(/\/$/, '');
// };

// const API_BASE_URL = getApiBaseUrl();

// console.log('API BASE URL:', API_BASE_URL);



const API_BASE_URL = getApiBaseUrl();

function getAuthToken(): string | null {
  if (typeof window !== 'undefined') {
    return localStorage.getItem('token');
  }
  return null;
}

function normalizeUserRole(role: any): string {
  if (role === 0 || role === '0' || role === 'Admin') return 'Admin';
  if (role === 1 || role === '1' || role === 'Teacher') return 'Teacher';
  if (role === 2 || role === '2' || role === 'Student') return 'Student';
  return String(role || 'Student');
}

function normalizeAssignmentStatus(status: any): string {
  if (status === 0 || status === '0' || status === 'Draft') return 'Draft';
  if (status === 1 || status === '1' || status === 'Published') return 'Published';
  return String(status || 'Draft');
}

function normalizeSubmissionStatus(status: any): string {
  if (status === 0 || status === '0' || status === 'Submitted') return 'Submitted';
  if (status === 1 || status === '1' || status === 'Late') return 'Late';
  if (status === 2 || status === '2' || status === 'Graded') return 'Graded';
  if (status === 3 || status === '3' || status === 'ResubmissionRequired') return 'ResubmissionRequired';
  return String(status || 'Submitted');
}

export function normalizeApiResponseData<T>(data: any): any {
  if (data === null || data === undefined) return data;

  if (Array.isArray(data)) {
    return data.map((item) => normalizeApiResponseData(item));
  }

  if (typeof data === 'object') {
    const copy: any = { ...data };

    // Handle user role
    if ('role' in copy) {
      copy.role = normalizeUserRole(copy.role);
    } else if ('Role' in copy) {
      copy.role = normalizeUserRole(copy.Role);
    }

    // Handle nested user object in AuthResponse
    if (copy.user && typeof copy.user === 'object') {
      copy.user = normalizeApiResponseData(copy.user);
    }

    // Handle status for Assignment or Submission
    if ('status' in copy || 'Status' in copy) {
      const rawStatus = copy.status !== undefined ? copy.status : copy.Status;
      if ('submissionsCount' in copy || 'maxMarks' in copy && 'deadline' in copy) {
        copy.status = normalizeAssignmentStatus(rawStatus);
      } else if ('studentId' in copy || 'answerText' in copy || 'marksObtained' in copy || 'assignmentId' in copy) {
        copy.status = normalizeSubmissionStatus(rawStatus);
      } else {
        if (rawStatus === 0 || rawStatus === 'Draft' || rawStatus === 1 && 'publishImmediately' in copy) {
          copy.status = normalizeAssignmentStatus(rawStatus);
        } else {
          copy.status = normalizeSubmissionStatus(rawStatus);
        }
      }
    }

    return copy;
  }

  return data;
}

export async function apiRequest<T>(
  endpoint: string,
  options: RequestInit = {}
): Promise<ApiResponse<T>> {
  const token = getAuthToken();

  const headers: Record<string, string> = {
    'Content-Type': 'application/json',
    ...(options.headers as Record<string, string>),
  };

  if (token) {
    headers['Authorization'] = `Bearer ${token}`;
  }

  let response: Response;
  try {
    response = await fetch(`${API_BASE_URL}${endpoint}`, {
      ...options,
      headers,
    });
  } catch (err: any) {
    return {
      success: false,
      statusCode: 503,
      message: err.message || 'Unable to connect to the backend server.',
      data: null as unknown as T,
    };
  }

  let rawData: any = null;
  const text = await response.text();
  if (text && text.trim().length > 0) {
    try {
      rawData = JSON.parse(text);
    } catch {
      rawData = null;
    }
  }

  let success = response.ok;
  let statusCode = response.status;
  let message = '';
  let payload: any = null;

  if (rawData && typeof rawData === 'object') {
    const rawSuccess = rawData.success ?? rawData.Success;
    const rawStatusCode = rawData.statusCode ?? rawData.StatusCode;
    const rawMessage = rawData.message ?? rawData.Message;
    const rawPayload = rawData.data !== undefined ? rawData.data : rawData.Data;

    if (typeof rawSuccess === 'boolean') {
      success = response.ok && rawSuccess;
    }
    if (typeof rawStatusCode === 'number') {
      statusCode = rawStatusCode;
    }
    if (typeof rawMessage === 'string' && rawMessage.trim()) {
      message = rawMessage;
    }

    if (rawPayload !== undefined) {
      payload = rawPayload;
    } else if (rawSuccess === undefined && !('errors' in rawData) && !('title' in rawData)) {
      payload = rawData;
    }

    // Support ASP.NET ProblemDetails / Validation error format
    if (!success && !message) {
      if (rawData.title && typeof rawData.title === 'string') {
        message = rawData.title;
      }
      if (rawData.errors && typeof rawData.errors === 'object') {
        const errorList: string[] = [];
        for (const [key, val] of Object.entries(rawData.errors)) {
          if (Array.isArray(val)) {
            errorList.push(`${key}: ${val.join(', ')}`);
          } else if (typeof val === 'string') {
            errorList.push(`${key}: ${val}`);
          }
        }
        if (errorList.length > 0) {
          message = message ? `${message} (${errorList.join('; ')})` : errorList.join('; ');
        }
      }
      if (rawData.detail && typeof rawData.detail === 'string') {
        message = message ? `${message} - ${rawData.detail}` : rawData.detail;
      }
    }
  }

  if (!message) {
    if (!response.ok) {
      message = `HTTP ${response.status}: ${response.statusText || 'Request failed'}`;
    } else {
      message = 'Success';
    }
  }

  const normalizedPayload = normalizeApiResponseData<T>(payload);

  return {
    success,
    statusCode,
    message,
    data: normalizedPayload as T,
  };
}

export const api = {
  // Auth
  login: (email: string, password: string) =>
    apiRequest<{ token: string; user: any }>('/auth/login', {
      method: 'POST',
      body: JSON.stringify({ email, password }),
    }),
  getCurrentUser: () => apiRequest<any>('/auth/me'),

  // Users (Admin)
  getUsers: () => apiRequest<any[]>('/users'),
  createUser: (user: any) =>
    apiRequest<any>('/users', {
      method: 'POST',
      body: JSON.stringify(user),
    }),
  updateUser: (id: string, user: any) =>
    apiRequest<any>(`/users/${id}`, {
      method: 'PUT',
      body: JSON.stringify(user),
    }),
  deactivateUser: (id: string) =>
    apiRequest<any>(`/users/${id}`, {
      method: 'DELETE',
    }),

  // Academic (Classes & Subjects)
  getClasses: () => apiRequest<any[]>('/classes'),
  createClass: (data: { name: string; description?: string }) =>
    apiRequest<any>('/classes', {
      method: 'POST',
      body: JSON.stringify(data),
    }),

  getSubjects: (classCourseId?: string) =>
    apiRequest<any[]>(`/subjects${classCourseId ? `?classCourseId=${classCourseId}` : ''}`),
  createSubject: (data: { name: string; classCourseId: string }) =>
    apiRequest<any>('/subjects', {
      method: 'POST',
      body: JSON.stringify(data),
    }),

  getTeacherAssignments: (teacherId?: string) =>
    apiRequest<any[]>(`/teacherassignments${teacherId ? `?teacherId=${teacherId}` : ''}`),
  assignTeacher: (data: { teacherId: string; subjectId: string; classCourseId: string }) =>
    apiRequest<any>('/teacherassignments', {
      method: 'POST',
      body: JSON.stringify(data),
    }),

  getStudentEnrollments: (studentId?: string, classCourseId?: string) => {
    const query = new URLSearchParams();
    if (studentId) query.append('studentId', studentId);
    if (classCourseId) query.append('classCourseId', classCourseId);
    return apiRequest<any[]>(`/studentenrollments?${query.toString()}`);
  },
  enrollStudent: (data: { studentId: string; classCourseId: string }) =>
    apiRequest<any>('/studentenrollments', {
      method: 'POST',
      body: JSON.stringify(data),
    }),

  // Assignments
  getAssignments: (classCourseId?: string, subjectId?: string) => {
    const query = new URLSearchParams();
    if (classCourseId) query.append('classCourseId', classCourseId);
    if (subjectId) query.append('subjectId', subjectId);
    return apiRequest<any[]>(`/assignments?${query.toString()}`);
  },
  getAssignmentById: (id: string) => apiRequest<any>(`/assignments/${id}`),
  createAssignment: (assignment: any) => {
    const formatted = {
      ...assignment,
      deadline: assignment.deadline ? new Date(assignment.deadline).toISOString() : assignment.deadline,
    };
    return apiRequest<any>('/assignments', {
      method: 'POST',
      body: JSON.stringify(formatted),
    });
  },
  updateAssignment: (id: string, assignment: any) => {
    const formatted = {
      ...assignment,
      deadline: assignment.deadline ? new Date(assignment.deadline).toISOString() : assignment.deadline,
    };
    return apiRequest<any>(`/assignments/${id}`, {
      method: 'PUT',
      body: JSON.stringify(formatted),
    });
  },
  publishAssignment: (id: string) =>
    apiRequest<any>(`/assignments/${id}/publish`, {
      method: 'PUT',
    }),
  deleteAssignment: (id: string) =>
    apiRequest<any>(`/assignments/${id}`, {
      method: 'DELETE',
    }),

  // Submissions
  submitAnswer: (assignmentId: string, submission: { answerText?: string; fileUrl?: string }) =>
    apiRequest<any>(`/assignments/${assignmentId}/submissions`, {
      method: 'POST',
      body: JSON.stringify({ assignmentId, ...submission }),
    }),
  getSubmissionsForAssignment: (assignmentId: string) =>
    apiRequest<any[]>(`/assignments/${assignmentId}/submissions`),
  getMySubmissions: () => apiRequest<any[]>('/submissions/my'),
  getSubmissionById: (id: string) => apiRequest<any>(`/submissions/${id}`),
  updateSubmission: (id: string, submission: { answerText?: string; fileUrl?: string }) =>
    apiRequest<any>(`/submissions/${id}`, {
      method: 'PUT',
      body: JSON.stringify(submission),
    }),
  gradeSubmission: (id: string, grade: { marksObtained: number; feedback?: string; status?: string }) =>
    apiRequest<any>(`/submissions/${id}/grade`, {
      method: 'PUT',
      body: JSON.stringify(grade),
    }),
};

