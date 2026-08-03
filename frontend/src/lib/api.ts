import { ApiResponse } from '@/types';

const API_BASE_URL = process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5000/api';

function getAuthToken(): string | null {
  if (typeof window !== 'undefined') {
    return localStorage.getItem('token');
  }
  return null;
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

  const response = await fetch(`${API_BASE_URL}${endpoint}`, {
    ...options,
    headers,
  });

  let data: ApiResponse<T>;
  try {
    data = await response.json();
  } catch (err) {
    throw new Error(`API response error: ${response.statusText}`);
  }

  if (!response.ok && !data.message) {
    data.message = `HTTP ${response.status}: ${response.statusText}`;
  }

  return data;
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
  createAssignment: (assignment: any) =>
    apiRequest<any>('/assignments', {
      method: 'POST',
      body: JSON.stringify(assignment),
    }),
  updateAssignment: (id: string, assignment: any) =>
    apiRequest<any>(`/assignments/${id}`, {
      method: 'PUT',
      body: JSON.stringify(assignment),
    }),
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
