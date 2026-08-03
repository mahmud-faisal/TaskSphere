'use client';

import React, { useEffect, useState } from 'react';
import { api } from '@/lib/api';
import { Assignment, Submission } from '@/types';
import { GraduationCap, Clock, FileCheck, CheckCircle2, AlertCircle, Edit3, Send, Award, FileText } from 'lucide-react';

export default function StudentDashboard() {
  const [assignments, setAssignments] = useState<Assignment[]>([]);
  const [mySubmissions, setMySubmissions] = useState<Submission[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [successMsg, setSuccessMsg] = useState<string | null>(null);

  // Submission Modal State
  const [selectedAssignment, setSelectedAssignment] = useState<Assignment | null>(null);
  const [existingSubmission, setExistingSubmission] = useState<Submission | null>(null);
  const [submissionData, setSubmissionData] = useState({ answerText: '', fileUrl: '' });
  const [submitting, setSubmitting] = useState(false);

  const fetchData = async () => {
    setLoading(true);
    setError(null);
    try {
      const [aRes, sRes] = await Promise.all([
        api.getAssignments(),
        api.getMySubmissions(),
      ]);

      if (aRes.success) setAssignments(aRes.data || []);
      if (sRes.success) setMySubmissions(sRes.data || []);
    } catch (err: any) {
      setError(err.message || 'Failed to fetch student dashboard data.');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchData();
  }, []);

  const openSubmitModal = (assignment: Assignment) => {
    setSelectedAssignment(assignment);
    const sub = mySubmissions.find((s) => s.assignmentId === assignment.id);
    if (sub) {
      setExistingSubmission(sub);
      setSubmissionData({ answerText: sub.answerText || '', fileUrl: sub.fileUrl || '' });
    } else {
      setExistingSubmission(null);
      setSubmissionData({ answerText: '', fileUrl: '' });
    }
  };

  const handleSubmitAnswer = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!selectedAssignment) return;
    setSubmitting(true);
    setError(null);

    try {
      let res;
      if (existingSubmission) {
        res = await api.updateSubmission(existingSubmission.id, submissionData);
      } else {
        res = await api.submitAnswer(selectedAssignment.id, submissionData);
      }

      if (res.success) {
        setSuccessMsg(existingSubmission ? 'Submission updated successfully!' : 'Answer submitted successfully!');
        setSelectedAssignment(null);
        fetchData();
      } else {
        setError(res.message);
      }
    } catch (err: any) {
      setError(err.message);
    } finally {
      setSubmitting(false);
    }
  };

  const getSubmissionForAssignment = (assignmentId: string) => {
    return mySubmissions.find((s) => s.assignmentId === assignmentId);
  };

  return (
    <div className="space-y-6">
      {/* Header Bar */}
      <div className="flex flex-col sm:flex-row items-start sm:items-center justify-between gap-4 bg-slate-900/60 backdrop-blur-md border border-slate-800 p-6 rounded-2xl">
        <div>
          <h1 className="text-2xl font-bold text-slate-100 flex items-center gap-2">
            <GraduationCap className="w-6 h-6 text-indigo-400" /> Student Portal
          </h1>
          <p className="text-xs text-slate-400 mt-1">View course assignments, submit your work, and check teacher feedback.</p>
        </div>
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

      {/* Assignments Grid */}
      <div className="space-y-4">
        <h2 className="text-lg font-bold text-slate-100">Course Assignments</h2>

        {loading ? (
          <div className="py-12 text-center text-slate-400 text-sm">Loading course assignments...</div>
        ) : assignments.length === 0 ? (
          <div className="bg-slate-900/40 border border-slate-800 rounded-2xl p-12 text-center">
            <p className="text-slate-400 text-sm">No published assignments available for your enrolled class yet.</p>
          </div>
        ) : (
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            {assignments.map((a) => {
              const sub = getSubmissionForAssignment(a.id);
              const isPastDeadline = new Date() > new Date(a.deadline);

              return (
                <div
                  key={a.id}
                  className="bg-slate-900/60 backdrop-blur-md border border-slate-800 hover:border-slate-700 p-5 rounded-2xl space-y-4 transition-all flex flex-col justify-between"
                >
                  <div className="space-y-2">
                    <div className="flex items-center justify-between">
                      <span className="text-xs text-indigo-400 font-semibold">{a.subjectName}</span>
                      <span className="text-xs text-slate-400 font-medium">Max Marks: {a.maxMarks}</span>
                    </div>

                    <h3 className="text-base font-bold text-slate-100">{a.title}</h3>
                    <p className="text-xs text-slate-400">{a.description || 'No description provided.'}</p>

                    <div className="pt-2 flex flex-wrap items-center justify-between text-xs text-slate-400 border-t border-slate-800/60">
                      <div className="flex items-center gap-1">
                        <Clock className="w-3.5 h-3.5 text-slate-500" />
                        <span>Deadline: {new Date(a.deadline).toLocaleString()}</span>
                      </div>
                      {isPastDeadline && (
                        <span className="text-amber-400 font-semibold">
                          {a.allowResubmission ? 'Late Allowed' : 'Closed'}
                        </span>
                      )}
                    </div>
                  </div>

                  {/* Submission Status & Feedback Display */}
                  {sub ? (
                    <div className="p-4 bg-slate-950/80 border border-slate-800 rounded-xl space-y-2">
                      <div className="flex items-center justify-between">
                        <span
                          className={`px-2.5 py-0.5 rounded-full text-xs font-semibold ${
                            sub.status === 'Graded'
                              ? 'bg-emerald-500/10 text-emerald-400 border border-emerald-500/20'
                              : sub.status === 'Late'
                              ? 'bg-amber-500/10 text-amber-400 border border-amber-500/20'
                              : 'bg-blue-500/10 text-blue-400 border border-blue-500/20'
                          }`}
                        >
                          Status: {sub.status}
                        </span>

                        {sub.status !== 'Graded' && (!isPastDeadline || a.allowResubmission) && (
                          <button
                            onClick={() => openSubmitModal(a)}
                            className="px-3 py-1 rounded-lg bg-slate-800 hover:bg-slate-700 text-indigo-400 text-xs font-semibold flex items-center gap-1 transition-colors"
                          >
                            <Edit3 className="w-3.5 h-3.5" /> Edit Submission
                          </button>
                        )}
                      </div>

                      {sub.status === 'Graded' && (
                        <div className="pt-2 border-t border-slate-800 space-y-1">
                          <div className="flex items-center gap-2 text-sm font-bold text-emerald-400">
                            <Award className="w-4 h-4" />
                            <span>
                              Grade: {sub.marksObtained} / {a.maxMarks}
                            </span>
                          </div>
                          {sub.feedback && (
                            <p className="text-xs text-slate-300 italic">Teacher Feedback: "{sub.feedback}"</p>
                          )}
                        </div>
                      )}
                    </div>
                  ) : (
                    <div className="pt-3 border-t border-slate-800/60 flex items-center justify-between">
                      <span className="text-xs text-slate-500">Not submitted yet</span>
                      <button
                        onClick={() => openSubmitModal(a)}
                        disabled={isPastDeadline && !a.allowResubmission}
                        className="px-4 py-2 rounded-xl bg-indigo-600 hover:bg-indigo-500 disabled:bg-slate-800 disabled:text-slate-600 text-white font-semibold text-xs flex items-center gap-1.5 transition-all shadow-md shadow-indigo-600/20"
                      >
                        <Send className="w-3.5 h-3.5" /> Submit Answer
                      </button>
                    </div>
                  )}
                </div>
              );
            })}
          </div>
        )}
      </div>

      {/* Submit / Edit Submission Modal */}
      {selectedAssignment && (
        <div className="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black/70 backdrop-blur-sm">
          <div className="bg-slate-900 border border-slate-800 rounded-2xl p-6 max-w-lg w-full shadow-2xl space-y-4">
            <h3 className="text-lg font-bold text-slate-100">
              {existingSubmission ? 'Edit Submission' : 'Submit Assignment'}
            </h3>
            <p className="text-xs text-indigo-400 font-medium">{selectedAssignment.title}</p>

            <form onSubmit={handleSubmitAnswer} className="space-y-4">
              <div>
                <label className="block text-xs font-medium text-slate-300 mb-1">Your Answer Text</label>
                <textarea
                  required={!submissionData.fileUrl}
                  placeholder="Type your complete solution or response here..."
                  value={submissionData.answerText}
                  onChange={(e) => setSubmissionData({ ...submissionData, answerText: e.target.value })}
                  className="w-full px-3 py-2 bg-slate-950 border border-slate-800 rounded-xl text-sm text-slate-100 focus:outline-none focus:border-indigo-500 h-32"
                />
              </div>

              <div>
                <label className="block text-xs font-medium text-slate-300 mb-1">File Attachment URL (Optional)</label>
                <input
                  type="url"
                  placeholder="https://example.com/my-solution.pdf"
                  value={submissionData.fileUrl}
                  onChange={(e) => setSubmissionData({ ...submissionData, fileUrl: e.target.value })}
                  className="w-full px-3 py-2 bg-slate-950 border border-slate-800 rounded-xl text-sm text-slate-100 focus:outline-none focus:border-indigo-500"
                />
              </div>

              <div className="flex justify-end gap-3 pt-2">
                <button
                  type="button"
                  onClick={() => setSelectedAssignment(null)}
                  className="px-4 py-2 text-xs font-semibold text-slate-400 hover:text-slate-200"
                >
                  Cancel
                </button>
                <button
                  type="submit"
                  disabled={submitting}
                  className="px-4 py-2 bg-indigo-600 hover:bg-indigo-500 text-white rounded-xl text-xs font-semibold shadow-md shadow-indigo-600/20 flex items-center gap-2"
                >
                  {submitting ? (
                    <div className="w-4 h-4 border-2 border-white/30 border-t-white rounded-full animate-spin" />
                  ) : existingSubmission ? (
                    'Update Submission'
                  ) : (
                    'Submit Answer'
                  )}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
}
