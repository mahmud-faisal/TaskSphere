'use client';

import React, { useEffect, useState } from 'react';
import { api } from '@/lib/api';
import { User, ClassCourse, Subject, TeacherAssignment, StudentEnrollment, UserRole } from '@/types';
import { Users, BookOpen, Layers, UserCheck, Plus, CheckCircle, XCircle, AlertCircle } from 'lucide-react';

export default function AdminDashboard() {
  const [activeTab, setActiveTab] = useState<'users' | 'academic' | 'teacher-assignments' | 'student-enrollments'>('users');
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [successMsg, setSuccessMsg] = useState<string | null>(null);

  // Data states
  const [users, setUsers] = useState<User[]>([]);
  const [classes, setClasses] = useState<ClassCourse[]>([]);
  const [subjects, setSubjects] = useState<Subject[]>([]);
  const [teacherAssignments, setTeacherAssignments] = useState<TeacherAssignment[]>([]);
  const [studentEnrollments, setStudentEnrollments] = useState<StudentEnrollment[]>([]);

  // Modal states
  const [showUserModal, setShowUserModal] = useState(false);
  const [showClassModal, setShowClassModal] = useState(false);
  const [showSubjectModal, setShowSubjectModal] = useState(false);
  const [showAssignModal, setShowAssignModal] = useState(false);
  const [showEnrollModal, setShowEnrollModal] = useState(false);

  // Form states
  const [newUser, setNewUser] = useState({ name: '', email: '', password: '', role: 'Teacher' as UserRole });
  const [newClass, setNewClass] = useState({ name: '', description: '' });
  const [newSubject, setNewSubject] = useState({ name: '', classCourseId: '' });
  const [newAssign, setNewAssign] = useState({ teacherId: '', subjectId: '', classCourseId: '' });
  const [newEnroll, setNewEnroll] = useState({ studentId: '', classCourseId: '' });

  const fetchData = async () => {
    setLoading(true);
    setError(null);
    try {
      const [uRes, cRes, sRes, taRes, seRes] = await Promise.all([
        api.getUsers(),
        api.getClasses(),
        api.getSubjects(),
        api.getTeacherAssignments(),
        api.getStudentEnrollments(),
      ]);

      if (uRes.success) setUsers(uRes.data || []);
      if (cRes.success) setClasses(cRes.data || []);
      if (sRes.success) setSubjects(sRes.data || []);
      if (taRes.success) setTeacherAssignments(taRes.data || []);
      if (seRes.success) setStudentEnrollments(seRes.data || []);
    } catch (err: any) {
      setError(err.message || 'Failed to load administrative data.');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchData();
  }, []);

  const handleCreateUser = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      const res = await api.createUser(newUser);
      if (res.success) {
        setSuccessMsg('User created successfully');
        setShowUserModal(false);
        setNewUser({ name: '', email: '', password: '', role: 'Teacher' });
        fetchData();
      } else {
        setError(res.message);
      }
    } catch (err: any) {
      setError(err.message);
    }
  };

  const handleDeactivateUser = async (id: string) => {
    if (!confirm('Are you sure you want to deactivate this user?')) return;
    try {
      const res = await api.deactivateUser(id);
      if (res.success) {
        setSuccessMsg('User deactivated');
        fetchData();
      } else {
        setError(res.message);
      }
    } catch (err: any) {
      setError(err.message);
    }
  };

  const handleCreateClass = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      const res = await api.createClass(newClass);
      if (res.success) {
        setSuccessMsg('Class created successfully');
        setShowClassModal(false);
        setNewClass({ name: '', description: '' });
        fetchData();
      } else {
        setError(res.message);
      }
    } catch (err: any) {
      setError(err.message);
    }
  };

  const handleCreateSubject = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      const res = await api.createSubject(newSubject);
      if (res.success) {
        setSuccessMsg('Subject created successfully');
        setShowSubjectModal(false);
        setNewSubject({ name: '', classCourseId: '' });
        fetchData();
      } else {
        setError(res.message);
      }
    } catch (err: any) {
      setError(err.message);
    }
  };

  const handleAssignTeacher = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      const res = await api.assignTeacher(newAssign);
      if (res.success) {
        setSuccessMsg('Teacher assigned successfully');
        setShowAssignModal(false);
        setNewAssign({ teacherId: '', subjectId: '', classCourseId: '' });
        fetchData();
      } else {
        setError(res.message);
      }
    } catch (err: any) {
      setError(err.message);
    }
  };

  const handleEnrollStudent = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      const res = await api.enrollStudent(newEnroll);
      if (res.success) {
        setSuccessMsg('Student enrolled successfully');
        setShowEnrollModal(false);
        setNewEnroll({ studentId: '', classCourseId: '' });
        fetchData();
      } else {
        setError(res.message);
      }
    } catch (err: any) {
      setError(err.message);
    }
  };

  const teachers = users.filter((u) => u.role === 'Teacher');
  const students = users.filter((u) => u.role === 'Student');

  return (
    <div className="space-y-6">
      {/* Header Stat Cards */}
      <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
        <div className="bg-slate-900/60 backdrop-blur-md border border-slate-800 p-5 rounded-2xl flex items-center gap-4">
          <div className="p-3 bg-purple-500/10 text-purple-400 rounded-xl border border-purple-500/20">
            <Users className="w-6 h-6" />
          </div>
          <div>
            <p className="text-xs text-slate-400 font-medium">Total Users</p>
            <h3 className="text-2xl font-bold text-slate-100">{users.length}</h3>
          </div>
        </div>

        <div className="bg-slate-900/60 backdrop-blur-md border border-slate-800 p-5 rounded-2xl flex items-center gap-4">
          <div className="p-3 bg-blue-500/10 text-blue-400 rounded-xl border border-blue-500/20">
            <BookOpen className="w-6 h-6" />
          </div>
          <div>
            <p className="text-xs text-slate-400 font-medium">Classes & Courses</p>
            <h3 className="text-2xl font-bold text-slate-100">{classes.length}</h3>
          </div>
        </div>

        <div className="bg-slate-900/60 backdrop-blur-md border border-slate-800 p-5 rounded-2xl flex items-center gap-4">
          <div className="p-3 bg-emerald-500/10 text-emerald-400 rounded-xl border border-emerald-500/20">
            <Layers className="w-6 h-6" />
          </div>
          <div>
            <p className="text-xs text-slate-400 font-medium">Subjects</p>
            <h3 className="text-2xl font-bold text-slate-100">{subjects.length}</h3>
          </div>
        </div>

        <div className="bg-slate-900/60 backdrop-blur-md border border-slate-800 p-5 rounded-2xl flex items-center gap-4">
          <div className="p-3 bg-amber-500/10 text-amber-400 rounded-xl border border-amber-500/20">
            <UserCheck className="w-6 h-6" />
          </div>
          <div>
            <p className="text-xs text-slate-400 font-medium">Active Enrollments</p>
            <h3 className="text-2xl font-bold text-slate-100">{studentEnrollments.length}</h3>
          </div>
        </div>
      </div>

      {/* Alert Messages */}
      {error && (
        <div className="p-4 rounded-xl bg-rose-500/10 border border-rose-500/20 text-rose-300 text-sm flex items-center justify-between">
          <div className="flex items-center gap-2">
            <AlertCircle className="w-5 h-5 text-rose-400" />
            <span>{error}</span>
          </div>
          <button onClick={() => setError(null)} className="text-rose-400 hover:text-rose-200">
            <XCircle className="w-4 h-4" />
          </button>
        </div>
      )}

      {successMsg && (
        <div className="p-4 rounded-xl bg-emerald-500/10 border border-emerald-500/20 text-emerald-300 text-sm flex items-center justify-between">
          <div className="flex items-center gap-2">
            <CheckCircle className="w-5 h-5 text-emerald-400" />
            <span>{successMsg}</span>
          </div>
          <button onClick={() => setSuccessMsg(null)} className="text-emerald-400 hover:text-emerald-200">
            <XCircle className="w-4 h-4" />
          </button>
        </div>
      )}

      {/* Tabs Bar */}
      <div className="flex items-center gap-2 border-b border-slate-800 pb-2">
        <button
          onClick={() => setActiveTab('users')}
          className={`px-4 py-2 rounded-xl text-sm font-medium transition-all ${
            activeTab === 'users'
              ? 'bg-indigo-600 text-white shadow-lg shadow-indigo-600/20'
              : 'text-slate-400 hover:text-slate-200 hover:bg-slate-900'
          }`}
        >
          Users Management
        </button>
        <button
          onClick={() => setActiveTab('academic')}
          className={`px-4 py-2 rounded-xl text-sm font-medium transition-all ${
            activeTab === 'academic'
              ? 'bg-indigo-600 text-white shadow-lg shadow-indigo-600/20'
              : 'text-slate-400 hover:text-slate-200 hover:bg-slate-900'
          }`}
        >
          Classes & Subjects
        </button>
        <button
          onClick={() => setActiveTab('teacher-assignments')}
          className={`px-4 py-2 rounded-xl text-sm font-medium transition-all ${
            activeTab === 'teacher-assignments'
              ? 'bg-indigo-600 text-white shadow-lg shadow-indigo-600/20'
              : 'text-slate-400 hover:text-slate-200 hover:bg-slate-900'
          }`}
        >
          Teacher Allocations
        </button>
        <button
          onClick={() => setActiveTab('student-enrollments')}
          className={`px-4 py-2 rounded-xl text-sm font-medium transition-all ${
            activeTab === 'student-enrollments'
              ? 'bg-indigo-600 text-white shadow-lg shadow-indigo-600/20'
              : 'text-slate-400 hover:text-slate-200 hover:bg-slate-900'
          }`}
        >
          Student Enrollments
        </button>
      </div>

      {/* Tab 1: Users Management */}
      {activeTab === 'users' && (
        <div className="space-y-4">
          <div className="flex items-center justify-between">
            <h2 className="text-lg font-bold text-slate-100">System Users</h2>
            <button
              onClick={() => setShowUserModal(true)}
              className="px-4 py-2 rounded-xl bg-indigo-600 hover:bg-indigo-500 text-white font-semibold text-xs flex items-center gap-2 transition-all shadow-md shadow-indigo-600/20"
            >
              <Plus className="w-4 h-4" /> Add User
            </button>
          </div>

          <div className="bg-slate-900/60 backdrop-blur-md border border-slate-800 rounded-2xl overflow-hidden shadow-xl">
            <table className="w-full text-left text-sm text-slate-300">
              <thead className="bg-slate-950/80 text-xs font-semibold text-slate-400 uppercase tracking-wider border-b border-slate-800">
                <tr>
                  <th className="py-3.5 px-4">Name</th>
                  <th className="py-3.5 px-4">Email</th>
                  <th className="py-3.5 px-4">Role</th>
                  <th className="py-3.5 px-4">Status</th>
                  <th className="py-3.5 px-4 text-right">Actions</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-slate-800/60">
                {users.map((u) => (
                  <tr key={u.id} className="hover:bg-slate-850/50 transition-colors">
                    <td className="py-3.5 px-4 font-medium text-slate-100">{u.name}</td>
                    <td className="py-3.5 px-4 text-slate-400">{u.email}</td>
                    <td className="py-3.5 px-4">
                      <span
                        className={`inline-flex px-2.5 py-0.5 rounded-full text-xs font-semibold ${
                          u.role === 'Admin'
                            ? 'bg-purple-500/10 text-purple-400 border border-purple-500/20'
                            : u.role === 'Teacher'
                            ? 'bg-emerald-500/10 text-emerald-400 border border-emerald-500/20'
                            : 'bg-blue-500/10 text-blue-400 border border-blue-500/20'
                        }`}
                      >
                        {u.role}
                      </span>
                    </td>
                    <td className="py-3.5 px-4">
                      {u.isActive ? (
                        <span className="text-emerald-400 font-medium text-xs">Active</span>
                      ) : (
                        <span className="text-slate-500 font-medium text-xs">Deactivated</span>
                      )}
                    </td>
                    <td className="py-3.5 px-4 text-right">
                      {u.isActive && u.role !== 'Admin' && (
                        <button
                          onClick={() => handleDeactivateUser(u.id)}
                          className="text-xs text-rose-400 hover:text-rose-300 font-medium px-2 py-1 rounded-lg hover:bg-rose-500/10 transition-colors"
                        >
                          Deactivate
                        </button>
                      )}
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
      )}

      {/* Tab 2: Academic (Classes & Subjects) */}
      {activeTab === 'academic' && (
        <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
          {/* Classes Column */}
          <div className="space-y-4">
            <div className="flex items-center justify-between">
              <h2 className="text-lg font-bold text-slate-100">Class / Courses</h2>
              <button
                onClick={() => setShowClassModal(true)}
                className="px-3.5 py-1.5 rounded-xl bg-indigo-600 hover:bg-indigo-500 text-white font-semibold text-xs flex items-center gap-1.5 transition-all shadow-md shadow-indigo-600/20"
              >
                <Plus className="w-3.5 h-3.5" /> Add Class
              </button>
            </div>
            <div className="space-y-3">
              {classes.map((c) => (
                <div key={c.id} className="bg-slate-900/60 backdrop-blur-md border border-slate-800 p-4 rounded-xl">
                  <h3 className="font-semibold text-slate-100">{c.name}</h3>
                  <p className="text-xs text-slate-400 mt-1">{c.description || 'No description provided.'}</p>
                </div>
              ))}
            </div>
          </div>

          {/* Subjects Column */}
          <div className="space-y-4">
            <div className="flex items-center justify-between">
              <h2 className="text-lg font-bold text-slate-100">Subjects</h2>
              <button
                onClick={() => setShowSubjectModal(true)}
                className="px-3.5 py-1.5 rounded-xl bg-indigo-600 hover:bg-indigo-500 text-white font-semibold text-xs flex items-center gap-1.5 transition-all shadow-md shadow-indigo-600/20"
              >
                <Plus className="w-3.5 h-3.5" /> Add Subject
              </button>
            </div>
            <div className="space-y-3">
              {subjects.map((s) => (
                <div key={s.id} className="bg-slate-900/60 backdrop-blur-md border border-slate-800 p-4 rounded-xl flex items-center justify-between">
                  <div>
                    <h3 className="font-semibold text-slate-100">{s.name}</h3>
                    <p className="text-xs text-indigo-400 mt-0.5">{s.classCourseName || 'Class Course'}</p>
                  </div>
                </div>
              ))}
            </div>
          </div>
        </div>
      )}

      {/* Tab 3: Teacher Allocations */}
      {activeTab === 'teacher-assignments' && (
        <div className="space-y-4">
          <div className="flex items-center justify-between">
            <h2 className="text-lg font-bold text-slate-100">Teacher Allocations</h2>
            <button
              onClick={() => setShowAssignModal(true)}
              className="px-4 py-2 rounded-xl bg-indigo-600 hover:bg-indigo-500 text-white font-semibold text-xs flex items-center gap-2 transition-all shadow-md shadow-indigo-600/20"
            >
              <Plus className="w-4 h-4" /> Assign Teacher
            </button>
          </div>

          <div className="bg-slate-900/60 backdrop-blur-md border border-slate-800 rounded-2xl overflow-hidden shadow-xl">
            <table className="w-full text-left text-sm text-slate-300">
              <thead className="bg-slate-950/80 text-xs font-semibold text-slate-400 uppercase tracking-wider border-b border-slate-800">
                <tr>
                  <th className="py-3.5 px-4">Teacher</th>
                  <th className="py-3.5 px-4">Subject</th>
                  <th className="py-3.5 px-4">Class / Course</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-slate-800/60">
                {teacherAssignments.map((ta) => (
                  <tr key={ta.id} className="hover:bg-slate-850/50 transition-colors">
                    <td className="py-3.5 px-4 font-medium text-slate-100">{ta.teacherName}</td>
                    <td className="py-3.5 px-4 text-indigo-400">{ta.subjectName}</td>
                    <td className="py-3.5 px-4 text-slate-300">{ta.classCourseName}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
      )}

      {/* Tab 4: Student Enrollments */}
      {activeTab === 'student-enrollments' && (
        <div className="space-y-4">
          <div className="flex items-center justify-between">
            <h2 className="text-lg font-bold text-slate-100">Student Enrollments</h2>
            <button
              onClick={() => setShowEnrollModal(true)}
              className="px-4 py-2 rounded-xl bg-indigo-600 hover:bg-indigo-500 text-white font-semibold text-xs flex items-center gap-2 transition-all shadow-md shadow-indigo-600/20"
            >
              <Plus className="w-4 h-4" /> Enroll Student
            </button>
          </div>

          <div className="bg-slate-900/60 backdrop-blur-md border border-slate-800 rounded-2xl overflow-hidden shadow-xl">
            <table className="w-full text-left text-sm text-slate-300">
              <thead className="bg-slate-950/80 text-xs font-semibold text-slate-400 uppercase tracking-wider border-b border-slate-800">
                <tr>
                  <th className="py-3.5 px-4">Student</th>
                  <th className="py-3.5 px-4">Email</th>
                  <th className="py-3.5 px-4">Class / Course</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-slate-800/60">
                {studentEnrollments.map((se) => (
                  <tr key={se.id} className="hover:bg-slate-850/50 transition-colors">
                    <td className="py-3.5 px-4 font-medium text-slate-100">{se.studentName}</td>
                    <td className="py-3.5 px-4 text-slate-400">{se.studentEmail}</td>
                    <td className="py-3.5 px-4 text-indigo-400">{se.classCourseName}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
      )}

      {/* Create User Modal */}
      {showUserModal && (
        <div className="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black/60 backdrop-blur-sm">
          <div className="bg-slate-900 border border-slate-800 rounded-2xl p-6 max-w-md w-full shadow-2xl space-y-4">
            <h3 className="text-lg font-bold text-slate-100">Create New User</h3>
            <form onSubmit={handleCreateUser} className="space-y-4">
              <div>
                <label className="block text-xs font-medium text-slate-300 mb-1">Full Name</label>
                <input
                  type="text"
                  required
                  value={newUser.name}
                  onChange={(e) => setNewUser({ ...newUser, name: e.target.value })}
                  className="w-full px-3 py-2 bg-slate-950 border border-slate-800 rounded-xl text-sm text-slate-100 focus:outline-none focus:border-indigo-500"
                />
              </div>
              <div>
                <label className="block text-xs font-medium text-slate-300 mb-1">Email</label>
                <input
                  type="email"
                  required
                  value={newUser.email}
                  onChange={(e) => setNewUser({ ...newUser, email: e.target.value })}
                  className="w-full px-3 py-2 bg-slate-950 border border-slate-800 rounded-xl text-sm text-slate-100 focus:outline-none focus:border-indigo-500"
                />
              </div>
              <div>
                <label className="block text-xs font-medium text-slate-300 mb-1">Password</label>
                <input
                  type="password"
                  required
                  value={newUser.password}
                  onChange={(e) => setNewUser({ ...newUser, password: e.target.value })}
                  className="w-full px-3 py-2 bg-slate-950 border border-slate-800 rounded-xl text-sm text-slate-100 focus:outline-none focus:border-indigo-500"
                />
              </div>
              <div>
                <label className="block text-xs font-medium text-slate-300 mb-1">Role</label>
                <select
                  value={newUser.role}
                  onChange={(e) => setNewUser({ ...newUser, role: e.target.value as UserRole })}
                  className="w-full px-3 py-2 bg-slate-950 border border-slate-800 rounded-xl text-sm text-slate-100 focus:outline-none focus:border-indigo-500"
                >
                  <option value="Teacher">Teacher</option>
                  <option value="Student">Student</option>
                  <option value="Admin">Admin</option>
                </select>
              </div>
              <div className="flex justify-end gap-3 pt-2">
                <button
                  type="button"
                  onClick={() => setShowUserModal(false)}
                  className="px-4 py-2 text-xs font-semibold text-slate-400 hover:text-slate-200"
                >
                  Cancel
                </button>
                <button
                  type="submit"
                  className="px-4 py-2 bg-indigo-600 hover:bg-indigo-500 text-white rounded-xl text-xs font-semibold shadow-md shadow-indigo-600/20"
                >
                  Create User
                </button>
              </div>
            </form>
          </div>
        </div>
      )}

      {/* Create Class Modal */}
      {showClassModal && (
        <div className="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black/60 backdrop-blur-sm">
          <div className="bg-slate-900 border border-slate-800 rounded-2xl p-6 max-w-md w-full shadow-2xl space-y-4">
            <h3 className="text-lg font-bold text-slate-100">Create Class / Course</h3>
            <form onSubmit={handleCreateClass} className="space-y-4">
              <div>
                <label className="block text-xs font-medium text-slate-300 mb-1">Class Name</label>
                <input
                  type="text"
                  required
                  placeholder="e.g. Grade 10 - Science & Tech"
                  value={newClass.name}
                  onChange={(e) => setNewClass({ ...newClass, name: e.target.value })}
                  className="w-full px-3 py-2 bg-slate-950 border border-slate-800 rounded-xl text-sm text-slate-100 focus:outline-none focus:border-indigo-500"
                />
              </div>
              <div>
                <label className="block text-xs font-medium text-slate-300 mb-1">Description</label>
                <textarea
                  value={newClass.description}
                  onChange={(e) => setNewClass({ ...newClass, description: e.target.value })}
                  className="w-full px-3 py-2 bg-slate-950 border border-slate-800 rounded-xl text-sm text-slate-100 focus:outline-none focus:border-indigo-500 h-24"
                />
              </div>
              <div className="flex justify-end gap-3 pt-2">
                <button
                  type="button"
                  onClick={() => setShowClassModal(false)}
                  className="px-4 py-2 text-xs font-semibold text-slate-400 hover:text-slate-200"
                >
                  Cancel
                </button>
                <button
                  type="submit"
                  className="px-4 py-2 bg-indigo-600 hover:bg-indigo-500 text-white rounded-xl text-xs font-semibold shadow-md shadow-indigo-600/20"
                >
                  Create Class
                </button>
              </div>
            </form>
          </div>
        </div>
      )}

      {/* Create Subject Modal */}
      {showSubjectModal && (
        <div className="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black/60 backdrop-blur-sm">
          <div className="bg-slate-900 border border-slate-800 rounded-2xl p-6 max-w-md w-full shadow-2xl space-y-4">
            <h3 className="text-lg font-bold text-slate-100">Create Subject</h3>
            <form onSubmit={handleCreateSubject} className="space-y-4">
              <div>
                <label className="block text-xs font-medium text-slate-300 mb-1">Subject Name</label>
                <input
                  type="text"
                  required
                  placeholder="e.g. Advanced Mathematics"
                  value={newSubject.name}
                  onChange={(e) => setNewSubject({ ...newSubject, name: e.target.value })}
                  className="w-full px-3 py-2 bg-slate-950 border border-slate-800 rounded-xl text-sm text-slate-100 focus:outline-none focus:border-indigo-500"
                />
              </div>
              <div>
                <label className="block text-xs font-medium text-slate-300 mb-1">Class / Course</label>
                <select
                  required
                  value={newSubject.classCourseId}
                  onChange={(e) => setNewSubject({ ...newSubject, classCourseId: e.target.value })}
                  className="w-full px-3 py-2 bg-slate-950 border border-slate-800 rounded-xl text-sm text-slate-100 focus:outline-none focus:border-indigo-500"
                >
                  <option value="">Select Class</option>
                  {classes.map((c) => (
                    <option key={c.id} value={c.id}>{c.name}</option>
                  ))}
                </select>
              </div>
              <div className="flex justify-end gap-3 pt-2">
                <button
                  type="button"
                  onClick={() => setShowSubjectModal(false)}
                  className="px-4 py-2 text-xs font-semibold text-slate-400 hover:text-slate-200"
                >
                  Cancel
                </button>
                <button
                  type="submit"
                  className="px-4 py-2 bg-indigo-600 hover:bg-indigo-500 text-white rounded-xl text-xs font-semibold shadow-md shadow-indigo-600/20"
                >
                  Create Subject
                </button>
              </div>
            </form>
          </div>
        </div>
      )}

      {/* Assign Teacher Modal */}
      {showAssignModal && (
        <div className="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black/60 backdrop-blur-sm">
          <div className="bg-slate-900 border border-slate-800 rounded-2xl p-6 max-w-md w-full shadow-2xl space-y-4">
            <h3 className="text-lg font-bold text-slate-100">Assign Teacher to Subject & Class</h3>
            <form onSubmit={handleAssignTeacher} className="space-y-4">
              <div>
                <label className="block text-xs font-medium text-slate-300 mb-1">Select Teacher</label>
                <select
                  required
                  value={newAssign.teacherId}
                  onChange={(e) => setNewAssign({ ...newAssign, teacherId: e.target.value })}
                  className="w-full px-3 py-2 bg-slate-950 border border-slate-800 rounded-xl text-sm text-slate-100 focus:outline-none focus:border-indigo-500"
                >
                  <option value="">Select Teacher</option>
                  {teachers.map((t) => (
                    <option key={t.id} value={t.id}>{t.name} ({t.email})</option>
                  ))}
                </select>
              </div>

              <div>
                <label className="block text-xs font-medium text-slate-300 mb-1">Select Class / Course</label>
                <select
                  required
                  value={newAssign.classCourseId}
                  onChange={(e) => setNewAssign({ ...newAssign, classCourseId: e.target.value })}
                  className="w-full px-3 py-2 bg-slate-950 border border-slate-800 rounded-xl text-sm text-slate-100 focus:outline-none focus:border-indigo-500"
                >
                  <option value="">Select Class</option>
                  {classes.map((c) => (
                    <option key={c.id} value={c.id}>{c.name}</option>
                  ))}
                </select>
              </div>

              <div>
                <label className="block text-xs font-medium text-slate-300 mb-1">Select Subject</label>
                <select
                  required
                  value={newAssign.subjectId}
                  onChange={(e) => setNewAssign({ ...newAssign, subjectId: e.target.value })}
                  className="w-full px-3 py-2 bg-slate-950 border border-slate-800 rounded-xl text-sm text-slate-100 focus:outline-none focus:border-indigo-500"
                >
                  <option value="">Select Subject</option>
                  {subjects
                    .filter((s) => !newAssign.classCourseId || s.classCourseId === newAssign.classCourseId)
                    .map((s) => (
                      <option key={s.id} value={s.id}>{s.name}</option>
                    ))}
                </select>
              </div>

              <div className="flex justify-end gap-3 pt-2">
                <button
                  type="button"
                  onClick={() => setShowAssignModal(false)}
                  className="px-4 py-2 text-xs font-semibold text-slate-400 hover:text-slate-200"
                >
                  Cancel
                </button>
                <button
                  type="submit"
                  className="px-4 py-2 bg-indigo-600 hover:bg-indigo-500 text-white rounded-xl text-xs font-semibold shadow-md shadow-indigo-600/20"
                >
                  Assign Teacher
                </button>
              </div>
            </form>
          </div>
        </div>
      )}

      {/* Enroll Student Modal */}
      {showEnrollModal && (
        <div className="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black/60 backdrop-blur-sm">
          <div className="bg-slate-900 border border-slate-800 rounded-2xl p-6 max-w-md w-full shadow-2xl space-y-4">
            <h3 className="text-lg font-bold text-slate-100">Enroll Student in Class</h3>
            <form onSubmit={handleEnrollStudent} className="space-y-4">
              <div>
                <label className="block text-xs font-medium text-slate-300 mb-1">Select Student</label>
                <select
                  required
                  value={newEnroll.studentId}
                  onChange={(e) => setNewEnroll({ ...newEnroll, studentId: e.target.value })}
                  className="w-full px-3 py-2 bg-slate-950 border border-slate-800 rounded-xl text-sm text-slate-100 focus:outline-none focus:border-indigo-500"
                >
                  <option value="">Select Student</option>
                  {students.map((st) => (
                    <option key={st.id} value={st.id}>{st.name} ({st.email})</option>
                  ))}
                </select>
              </div>

              <div>
                <label className="block text-xs font-medium text-slate-300 mb-1">Select Class / Course</label>
                <select
                  required
                  value={newEnroll.classCourseId}
                  onChange={(e) => setNewEnroll({ ...newEnroll, classCourseId: e.target.value })}
                  className="w-full px-3 py-2 bg-slate-950 border border-slate-800 rounded-xl text-sm text-slate-100 focus:outline-none focus:border-indigo-500"
                >
                  <option value="">Select Class</option>
                  {classes.map((c) => (
                    <option key={c.id} value={c.id}>{c.name}</option>
                  ))}
                </select>
              </div>

              <div className="flex justify-end gap-3 pt-2">
                <button
                  type="button"
                  onClick={() => setShowEnrollModal(false)}
                  className="px-4 py-2 text-xs font-semibold text-slate-400 hover:text-slate-200"
                >
                  Cancel
                </button>
                <button
                  type="submit"
                  className="px-4 py-2 bg-indigo-600 hover:bg-indigo-500 text-white rounded-xl text-xs font-semibold shadow-md shadow-indigo-600/20"
                >
                  Enroll Student
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
}
