'use client';

import React, { useEffect, useState } from 'react';
import { api } from '@/lib/api';
import { Assignment, Submission, ClassCourse, Subject, TeacherAssignment } from '@/types';
import { Plus, BookOpen, Clock, FileCheck, CheckCircle2, AlertCircle, Eye, Edit3, Trash2, Send } from 'lucide-react';

export default function TeacherDashboard() {
  const [assignments, setAssignments] = useState<Assignment[]>([]);
  const [teacherAllocations, setTeacherAllocations] = useState<TeacherAssignment[]>([]);
  const [classes, setClasses] = useState<ClassCourse[]>([]);
  const [subjects, setSubjects] = useState<Subject[]>([]);

  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [successMsg, setSuccessMsg] = useState<string | null>(null);

  // Assignment Modal State
  const [showCreateModal, setShowCreateModal] = useState(false);
  const [newAssignment, setNewAssignment] = useState({
    title: '',
    description: '',
    classCourseId: '',
    subjectId: '',
    deadline: '',
    maxMarks: 100,
    allowResubmission: false,
    publishImmediately: false,
  });

  // Submissions Modal State
  const [selectedAssignment, setSelectedAssignment] = useState<Assignment | null>(null);
  const [submissions, setSubmissions] = useState<Submission[]>([]);
  const [loadingSubmissions, setLoadingSubmissions] = useState(false);

  // Grade Modal State
  const [gradingSubmission, setGradingSubmission] = useState<Submission | null>(null);
  const [gradeData, setGradeData] = useState({ marksObtained: 0, feedback: '', status: 'Graded' });

  const fetchData = async () => {
    setLoading(true);
    setError(null);
    try {
      const [aRes, taRes, cRes, sRes] = await Promise.all([
        api.getAssignments(),
        api.getTeacherAssignments(),
        api.getClasses(),
        api.getSubjects(),
      ]);

      if (aRes.success) setAssignments(aRes.data || []);
      if (taRes.success) setTeacherAllocations(taRes.data || []);
      if (cRes.success) setClasses(cRes.data || []);
      if (sRes.success) setSubjects(sRes.data || []);
    } catch (err: any) {
      setError(err.message || 'Failed to fetch teacher dashboard data.');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchData();
  }, []);

  const handleCreateAssignment = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);
    try {
      const res = await api.createAssignment(newAssignment);
      if (res.success) {
        setSuccessMsg('Assignment created successfully');
        setShowCreateModal(false);
        setNewAssignment({
          title: '',
          description: '',
          classCourseId: '',
          subjectId: '',
          deadline: '',
          maxMarks: 100,
          allowResubmission: false,
          publishImmediately: false,
        });
        fetchData();
      } else {
        setError(res.message);
      }
    } catch (err: any) {
      setError(err.message);
    }
  };

  const handlePublishAssignment = async (id: string) => {
    try {
      const res = await api.publishAssignment(id);
      if (res.success) {
        setSuccessMsg('Assignment published to students!');
        fetchData();
      } else {
        setError(res.message);
      }
    } catch (err: any) {
      setError(err.message);
    }
  };

  const handleDeleteAssignment = async (id: string) => {
    if (!confirm('Are you sure you want to delete this assignment?')) return;
    try {
      const res = await api.deleteAssignment(id);
      if (res.success) {
        setSuccessMsg('Assignment deleted.');
        fetchData();
      } else {
        setError(res.message);
      }
    } catch (err: any) {
      setError(err.message);
    }
  };

  const handleViewSubmissions = async (assignment: Assignment) => {
    setSelectedAssignment(assignment);
    setLoadingSubmissions(true);
    try {
      const res = await api.getSubmissionsForAssignment(assignment.id);
      if (res.success) {
        setSubmissions(res.data || []);
      } else {
        setError(res.message);
      }
    } catch (err: any) {
      setError(err.message);
    } finally {
      setLoadingSubmissions(false);
    }
  };

  const handleGradeSubmission = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!gradingSubmission) return;
    try {
      const res = await api.gradeSubmission(gradingSubmission.id, gradeData);
      if (res.success) {
        setSuccessMsg('Submission graded successfully!');
        setGradingSubmission(null);
        if (selectedAssignment) {
          handleViewSubmissions(selectedAssignment);
        }
      } else {
        setError(res.message);
      }
    } catch (err: any) {
      setError(err.message);
    }
  };

  // Filter subjects based on selected class in create modal
  const availableClasses = classes;
  const availableSubjects = subjects.filter(
    (s) => !newAssignment.classCourseId || s.classCourseId === newAssignment.classCourseId
  );

  return (
    <div className="space-y-6">
      {/* Header Bar */}
      <div className="flex flex-col sm:flex-row items-start sm:items-center justify-between gap-4 bg-slate-900/60 backdrop-blur-md border border-slate-800 p-6 rounded-2xl">
        <div>
          <h1 className="text-2xl font-bold text-slate-100">Teacher Dashboard</h1>
          <p className="text-xs text-slate-400 mt-1">Manage course assignments, view student submissions, and award grades.</p>
        </div>
        <button
          onClick={() => setShowCreateModal(true)}
          className="px-4 py-2.5 rounded-xl bg-gradient-to-r from-indigo-500 to-purple-600 hover:from-indigo-600 hover:to-purple-700 text-white font-semibold text-xs flex items-center gap-2 transition-all shadow-lg shadow-indigo-500/20"
        >
          <Plus className="w-4 h-4" /> Create New Assignment
        </button>
      </div>

      {/* Alerts */}
      {error && (
        <div className="p-4 rounded-xl bg-rose-500/10 border border-rose-500/20 text-rose-300 text-sm flex items-center justify-between">
          <div className="flex items-center gap-2">
            <AlertCircle className="w-5 h-5 text-rose-400" />
            <span>{error}</span>
          </div>
          <button onClick={() => setError(null)} className="text-rose-400 hover:text-rose-200">
            ✕
          </button>
        </div>
      )}

      {successMsg && (
        <div className="p-4 rounded-xl bg-emerald-500/10 border border-emerald-500/20 text-emerald-300 text-sm flex items-center justify-between">
          <div className="flex items-center gap-2">
            <CheckCircle2 className="w-5 h-5 text-emerald-400" />
            <span>{successMsg}</span>
          </div>
          <button onClick={() => setSuccessMsg(null)} className="text-emerald-400 hover:text-emerald-200">
            ✕
          </button>
        </div>
      )}

      {/* Assignments List */}
      <div className="space-y-4">
        <h2 className="text-lg font-bold text-black-100 flex items-center gap-2">
          <BookOpen className="w-5 h-5 text-indigo-400" /> My Assignments
        </h2>

        {assignments.length === 0 ? (
          <div className="bg-slate-200/40 border border-slate-800 rounded-2xl p-12 text-center">
            <p className="text-slate-400 text-sm">No assignments created yet. Click "Create New Assignment" to get started.</p>
          </div>
        ) : (
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            {assignments.map((a) => (
              <div
                key={a.id}
                className="bg-slate-200/60 backdrop-blur-md border border-slate-800 hover:border-slate-700 p-5 rounded-2xl space-y-3 transition-all flex flex-col justify-between"
              >
                <div className="space-y-2">
                  <div className="flex items-center justify-between">
                    <span
                      className={`px-2.5 py-0.5 rounded-full text-xs font-semibold ${
                        a.status === 'Published'
                          ? 'bg-emerald-500/10 text-emerald-400 border border-emerald-500/20'
                          : 'bg-amber-500/10 text-amber-400 border border-amber-500/20'
                      }`}
                    >
                      {a.status}
                    </span>
                    <span className="text-xs font-medium text-slate-400">Max Marks: {a.maxMarks}</span>
                  </div>

                  <h3 className="text-base font-bold text-gray-700">{a.title}</h3>
                  <p className="text-xs text-slate-400 line-clamp-2">{a.description || 'No description.'}</p>

                  <div className="pt-2 flex flex-wrap gap-3 text-xs text-slate-400 border-t border-slate-800/60">
                    <div>
                      Class: <span className="text-slate-200 font-medium">{a.classCourseName}</span>
                    </div>
                    <div>
                      Subject: <span className="text-indigo-400 font-medium">{a.subjectName}</span>
                    </div>
                    <div className="flex items-center gap-1">
                      <Clock className="w-3.5 h-3.5 text-slate-500" />
                      <span>{new Date(a.deadline).toLocaleString()}</span>
                    </div>
                  </div>
                </div>

                <div className="pt-3 border-t border-slate-800/60 flex items-center justify-between gap-2">
                  <button
                    onClick={() => handleViewSubmissions(a)}
                    className="px-3 py-1.5 rounded-xl bg-slate-800 hover:bg-slate-700 text-slate-200 text-xs font-semibold flex items-center gap-1.5 transition-colors"
                  >
                    <Eye className="w-3.5 h-3.5" /> Submissions ({a.submissionsCount})
                  </button>

                  <div className="flex items-center gap-2">
                    {a.status === 'Draft' && (
                      <button
                        onClick={() => handlePublishAssignment(a.id)}
                        className="px-3 py-1.5 rounded-xl bg-emerald-600 hover:bg-emerald-500 text-white text-xs font-semibold flex items-center gap-1 transition-colors shadow-sm shadow-emerald-600/20"
                      >
                        <Send className="w-3.5 h-3.5" /> Publish
                      </button>
                    )}
                    <button
                      onClick={() => handleDeleteAssignment(a.id)}
                      className="p-1.5 rounded-lg text-rose-400 hover:bg-rose-500/10 transition-colors"
                      title="Delete assignment"
                    >
                      <Trash2 className="w-4 h-4" />
                    </button>
                  </div>
                </div>
              </div>
            ))}
          </div>
        )}
      </div>

      {/* Create Assignment Modal */}
      {showCreateModal && (
        <div className="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black/60 backdrop-blur-sm">
          <div className="bg-slate-900 border border-slate-800 rounded-2xl p-6 max-w-lg w-full shadow-2xl space-y-4 max-h-[90vh] overflow-y-auto">
            <h3 className="text-lg font-bold text-slate-100">Create New Assignment</h3>
            <form onSubmit={handleCreateAssignment} className="space-y-4">
              <div>
                <label className="block text-xs font-medium text-slate-300 mb-1">Title</label>
                <input
                  type="text"
                  required
                  placeholder="Assignment Title"
                  value={newAssignment.title}
                  onChange={(e) => setNewAssignment({ ...newAssignment, title: e.target.value })}
                  className="w-full px-3 py-2 bg-slate-950 border border-slate-800 rounded-xl text-sm text-slate-100 focus:outline-none focus:border-indigo-500"
                />
              </div>

              <div>
                <label className="block text-xs font-medium text-slate-300 mb-1">Description</label>
                <textarea
                  placeholder="Instructions or problem set description"
                  value={newAssignment.description}
                  onChange={(e) => setNewAssignment({ ...newAssignment, description: e.target.value })}
                  className="w-full px-3 py-2 bg-slate-950 border border-slate-800 rounded-xl text-sm text-slate-100 focus:outline-none focus:border-indigo-500 h-24"
                />
              </div>

              <div className="grid grid-cols-2 gap-3">
                <div>
                  <label className="block text-xs font-medium text-slate-300 mb-1">Class / Course</label>
                  <select
                    required
                    value={newAssignment.classCourseId}
                    onChange={(e) => setNewAssignment({ ...newAssignment, classCourseId: e.target.value })}
                    className="w-full px-3 py-2 bg-slate-950 border border-slate-800 rounded-xl text-sm text-slate-100 focus:outline-none focus:border-indigo-500"
                  >
                    <option value="">Select Class</option>
                    {availableClasses.map((c) => (
                      <option key={c.id} value={c.id}>{c.name}</option>
                    ))}
                  </select>
                </div>

                <div>
                  <label className="block text-xs font-medium text-slate-300 mb-1">Subject</label>
                  <select
                    required
                    value={newAssignment.subjectId}
                    onChange={(e) => setNewAssignment({ ...newAssignment, subjectId: e.target.value })}
                    className="w-full px-3 py-2 bg-slate-950 border border-slate-800 rounded-xl text-sm text-slate-100 focus:outline-none focus:border-indigo-500"
                  >
                    <option value="">Select Subject</option>
                    {availableSubjects.map((s) => (
                      <option key={s.id} value={s.id}>{s.name}</option>
                    ))}
                  </select>
                </div>
              </div>

              <div className="grid grid-cols-2 gap-3">
                <div>
                  <label className="block text-xs font-medium text-slate-300 mb-1">Deadline</label>
                  <input
                    type="datetime-local"
                    required
                    value={newAssignment.deadline}
                    onChange={(e) => setNewAssignment({ ...newAssignment, deadline: e.target.value })}
                    className="w-full px-3 py-2 bg-slate-950 border border-slate-800 rounded-xl text-sm text-slate-100 focus:outline-none focus:border-indigo-500"
                  />
                </div>

                <div>
                  <label className="block text-xs font-medium text-slate-300 mb-1">Maximum Marks</label>
                  <input
                    type="number"
                    min="1"
                    required
                    value={newAssignment.maxMarks}
                    onChange={(e) => setNewAssignment({ ...newAssignment, maxMarks: parseInt(e.target.value) || 100 })}
                    className="w-full px-3 py-2 bg-slate-950 border border-slate-800 rounded-xl text-sm text-slate-100 focus:outline-none focus:border-indigo-500"
                  />
                </div>
              </div>

              <div className="flex items-center gap-6 pt-2">
                <label className="flex items-center gap-2 text-xs font-medium text-slate-300 cursor-pointer">
                  <input
                    type="checkbox"
                    checked={newAssignment.allowResubmission}
                    onChange={(e) => setNewAssignment({ ...newAssignment, allowResubmission: e.target.checked })}
                    className="rounded bg-slate-950 border-slate-800 text-indigo-600 focus:ring-0"
                  />
                  Allow Resubmission
                </label>

                <label className="flex items-center gap-2 text-xs font-medium text-slate-300 cursor-pointer">
                  <input
                    type="checkbox"
                    checked={newAssignment.publishImmediately}
                    onChange={(e) => setNewAssignment({ ...newAssignment, publishImmediately: e.target.checked })}
                    className="rounded bg-slate-950 border-slate-800 text-emerald-600 focus:ring-0"
                  />
                  Publish Immediately
                </label>
              </div>

              <div className="flex justify-end gap-3 pt-4 border-t border-slate-800">
                <button
                  type="button"
                  onClick={() => setShowCreateModal(false)}
                  className="px-4 py-2 text-xs font-semibold text-slate-400 hover:text-slate-200"
                >
                  Cancel
                </button>
                <button
                  type="submit"
                  className="px-4 py-2 bg-indigo-600 hover:bg-indigo-500 text-white rounded-xl text-xs font-semibold shadow-md shadow-indigo-600/20"
                >
                  Save Assignment
                </button>
              </div>
            </form>
          </div>
        </div>
      )}

      {/* View Submissions Modal */}
      {selectedAssignment && (
        <div className="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black/70 backdrop-blur-sm">
          <div className="bg-slate-900 border border-slate-800 rounded-2xl p-6 max-w-4xl w-full shadow-2xl space-y-4 max-h-[90vh] overflow-y-auto">
            <div className="flex items-center justify-between pb-3 border-b border-slate-800">
              <div>
                <h3 className="text-lg font-bold text-slate-100">{selectedAssignment.title}</h3>
                <p className="text-xs text-slate-400">Submissions List • Max Marks: {selectedAssignment.maxMarks}</p>
              </div>
              <button
                onClick={() => setSelectedAssignment(null)}
                className="px-3 py-1.5 rounded-lg bg-slate-800 hover:bg-slate-700 text-slate-300 text-xs font-semibold"
              >
                Close
              </button>
            </div>

            {loadingSubmissions ? (
              <div className="py-12 text-center text-slate-400 text-sm">Loading student submissions...</div>
            ) : submissions.length === 0 ? (
              <div className="py-12 text-center text-slate-400 text-sm">No student submissions received for this assignment yet.</div>
            ) : (
              <div className="space-y-4">
                {submissions.map((sub) => (
                  <div key={sub.id} className="bg-slate-950/80 border border-slate-800 p-4 rounded-xl space-y-3">
                    <div className="flex items-center justify-between">
                      <div>
                        <h4 className="font-bold text-slate-100 text-sm">{sub.studentName}</h4>
                        <p className="text-xs text-slate-400">{sub.studentEmail}</p>
                      </div>
                      <div className="flex items-center gap-3">
                        <span
                          className={`px-2.5 py-0.5 rounded-full text-xs font-semibold ${
                            sub.status === 'Graded'
                              ? 'bg-emerald-500/10 text-emerald-400 border border-emerald-500/20'
                              : sub.status === 'Late'
                              ? 'bg-rose-500/10 text-rose-400 border border-rose-500/20'
                              : 'bg-blue-500/10 text-blue-400 border border-blue-500/20'
                          }`}
                        >
                          {sub.status}
                        </span>
                        <button
                          onClick={() => {
                            setGradingSubmission(sub);
                            setGradeData({
                              marksObtained: sub.marksObtained ?? 0,
                              feedback: sub.feedback ?? '',
                              status: 'Graded',
                            });
                          }}
                          className="px-3 py-1.5 rounded-xl bg-indigo-600 hover:bg-indigo-500 text-white text-xs font-semibold shadow-sm shadow-indigo-600/20"
                        >
                          {sub.status === 'Graded' ? 'Edit Grade' : 'Grade Submission'}
                        </button>
                      </div>
                    </div>

                    <div className="p-3 bg-slate-900 rounded-lg text-xs text-slate-300 font-mono whitespace-pre-wrap">
                      {sub.answerText || 'No text content.'}
                    </div>

                    {sub.fileUrl && (
                      <div className="text-xs text-indigo-400">
                        Attachment:{' '}
                        <a href={sub.fileUrl} target="_blank" rel="noopener noreferrer" className="underline hover:text-indigo-300">
                          {sub.fileUrl}
                        </a>
                      </div>
                    )}

                    {sub.status === 'Graded' && (
                      <div className="p-3 bg-emerald-500/5 border border-emerald-500/20 rounded-lg text-xs space-y-1">
                        <div className="font-semibold text-emerald-400">
                          Marks: {sub.marksObtained} / {selectedAssignment.maxMarks}
                        </div>
                        {sub.feedback && <div className="text-slate-300">Feedback: {sub.feedback}</div>}
                      </div>
                    )}
                  </div>
                ))}
              </div>
            )}
          </div>
        </div>
      )}

      {/* Grade Submission Modal */}
      {gradingSubmission && (
        <div className="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black/80 backdrop-blur-sm">
          <div className="bg-slate-900 border border-slate-800 rounded-2xl p-6 max-w-md w-full shadow-2xl space-y-4">
            <h3 className="text-lg font-bold text-slate-100">Grade Student Answer</h3>
            <p className="text-xs text-slate-400">Student: {gradingSubmission.studentName}</p>

            <form onSubmit={handleGradeSubmission} className="space-y-4">
              <div>
                <label className="block text-xs font-medium text-slate-300 mb-1">Marks Obtained (Max: {selectedAssignment?.maxMarks})</label>
                <input
                  type="number"
                  min="0"
                  max={selectedAssignment?.maxMarks}
                  required
                  value={gradeData.marksObtained}
                  onChange={(e) => setGradeData({ ...gradeData, marksObtained: parseInt(e.target.value) || 0 })}
                  className="w-full px-3 py-2 bg-slate-950 border border-slate-800 rounded-xl text-sm text-slate-100 focus:outline-none focus:border-indigo-500"
                />
              </div>

              <div>
                <label className="block text-xs font-medium text-slate-300 mb-1">Feedback</label>
                <textarea
                  placeholder="Provide constructive feedback for the student..."
                  value={gradeData.feedback}
                  onChange={(e) => setGradeData({ ...gradeData, feedback: e.target.value })}
                  className="w-full px-3 py-2 bg-slate-950 border border-slate-800 rounded-xl text-sm text-slate-100 focus:outline-none focus:border-indigo-500 h-24"
                />
              </div>

              <div className="flex justify-end gap-3 pt-2">
                <button
                  type="button"
                  onClick={() => setGradingSubmission(null)}
                  className="px-4 py-2 text-xs font-semibold text-slate-400 hover:text-slate-200"
                >
                  Cancel
                </button>
                <button
                  type="submit"
                  className="px-4 py-2 bg-emerald-600 hover:bg-emerald-500 text-white rounded-xl text-xs font-semibold shadow-md shadow-emerald-600/20"
                >
                  Submit Grade
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
}
