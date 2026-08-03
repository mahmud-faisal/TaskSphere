export type UserRole = 'Admin' | 'Teacher' | 'Student';

export interface ApiResponse<T> {
  success: boolean;
  statusCode: number;
  message: string;
  data: T;
}

export interface User {
  id: string;
  name: string;
  email: string;
  role: UserRole;
  createdAt: string;
  isActive: boolean;
}

export interface AuthResponse {
  token: string;
  user: User;
}

export interface ClassCourse {
  id: string;
  name: string;
  description?: string;
}

export interface Subject {
  id: string;
  name: string;
  classCourseId: string;
  classCourseName?: string;
}

export interface TeacherAssignment {
  id: string;
  teacherId: string;
  teacherName?: string;
  subjectId: string;
  subjectName?: string;
  classCourseId: string;
  classCourseName?: string;
}

export interface StudentEnrollment {
  id: string;
  studentId: string;
  studentName?: string;
  studentEmail?: string;
  classCourseId: string;
  classCourseName?: string;
}

export type AssignmentStatus = 'Draft' | 'Published';

export interface Assignment {
  id: string;
  title: string;
  description?: string;
  subjectId: string;
  subjectName?: string;
  classCourseId: string;
  classCourseName?: string;
  teacherId: string;
  teacherName?: string;
  deadline: string;
  maxMarks: number;
  status: AssignmentStatus;
  allowResubmission: boolean;
  createdAt: string;
  updatedAt: string;
  submissionsCount: number;
}

export type SubmissionStatus = 'Submitted' | 'Late' | 'Graded' | 'ResubmissionRequired';

export interface Submission {
  id: string;
  assignmentId: string;
  assignmentTitle?: string;
  maxMarks: number;
  studentId: string;
  studentName?: string;
  studentEmail?: string;
  answerText?: string;
  fileUrl?: string;
  submittedAt: string;
  updatedAt: string;
  status: SubmissionStatus;
  marksObtained?: number;
  feedback?: string;
  gradedAt?: string;
  gradedByTeacherId?: string;
  gradedByTeacherName?: string;
}
