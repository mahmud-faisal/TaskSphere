-- Assignment & Submission Management System Schema
-- PostgreSQL DDL Script

-- Drop existing tables if they exist (in reverse order of dependencies)
DROP TABLE IF EXISTS "Submissions" CASCADE;
DROP TABLE IF EXISTS "Assignments" CASCADE;
DROP TABLE IF EXISTS "StudentEnrollments" CASCADE;
DROP TABLE IF EXISTS "TeacherSubjectAssignments" CASCADE;
DROP TABLE IF EXISTS "Subjects" CASCADE;
DROP TABLE IF EXISTS "ClassCourses" CASCADE;
DROP TABLE IF EXISTS "Users" CASCADE;

-- Enable UUID extension
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- 1. Users Table
CREATE TABLE "Users" (
    "Id" UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    "Name" VARCHAR(100) NOT NULL,
    "Email" VARCHAR(150) NOT NULL UNIQUE,
    "PasswordHash" VARCHAR(255) NOT NULL,
    "Role" VARCHAR(20) NOT NULL CHECK ("Role" IN ('Admin', 'Teacher', 'Student')),
    "CreatedAt" TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "IsActive" BOOLEAN NOT NULL DEFAULT TRUE
);

-- 2. ClassCourses Table
CREATE TABLE "ClassCourses" (
    "Id" UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    "Name" VARCHAR(100) NOT NULL,
    "Description" TEXT NULL
);

-- 3. Subjects Table
CREATE TABLE "Subjects" (
    "Id" UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    "Name" VARCHAR(100) NOT NULL,
    "ClassCourseId" UUID NOT NULL REFERENCES "ClassCourses"("Id") ON DELETE CASCADE
);

-- 4. TeacherSubjectAssignments Table
CREATE TABLE "TeacherSubjectAssignments" (
    "Id" UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    "TeacherId" UUID NOT NULL REFERENCES "Users"("Id") ON DELETE CASCADE,
    "SubjectId" UUID NOT NULL REFERENCES "Subjects"("Id") ON DELETE CASCADE,
    "ClassCourseId" UUID NOT NULL REFERENCES "ClassCourses"("Id") ON DELETE CASCADE,
    CONSTRAINT "UQ_TeacherSubjectAssignment" UNIQUE ("TeacherId", "SubjectId", "ClassCourseId")
);

-- 5. StudentEnrollments Table
CREATE TABLE "StudentEnrollments" (
    "Id" UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    "StudentId" UUID NOT NULL REFERENCES "Users"("Id") ON DELETE CASCADE,
    "ClassCourseId" UUID NOT NULL REFERENCES "ClassCourses"("Id") ON DELETE CASCADE,
    CONSTRAINT "UQ_StudentEnrollment" UNIQUE ("StudentId", "ClassCourseId")
);

-- 6. Assignments Table
CREATE TABLE "Assignments" (
    "Id" UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    "Title" VARCHAR(200) NOT NULL,
    "Description" TEXT NULL,
    "SubjectId" UUID NOT NULL REFERENCES "Subjects"("Id") ON DELETE CASCADE,
    "ClassCourseId" UUID NOT NULL REFERENCES "ClassCourses"("Id") ON DELETE CASCADE,
    "TeacherId" UUID NOT NULL REFERENCES "Users"("Id") ON DELETE CASCADE,
    "Deadline" TIMESTAMPTZ NOT NULL,
    "MaxMarks" INT NOT NULL CHECK ("MaxMarks" > 0),
    "Status" VARCHAR(20) NOT NULL DEFAULT 'Draft' CHECK ("Status" IN ('Draft', 'Published')),
    "AllowResubmission" BOOLEAN NOT NULL DEFAULT FALSE,
    "CreatedAt" TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "UpdatedAt" TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- 7. Submissions Table
CREATE TABLE "Submissions" (
    "Id" UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    "AssignmentId" UUID NOT NULL REFERENCES "Assignments"("Id") ON DELETE CASCADE,
    "StudentId" UUID NOT NULL REFERENCES "Users"("Id") ON DELETE CASCADE,
    "AnswerText" TEXT NULL,
    "FileUrl" VARCHAR(500) NULL,
    "SubmittedAt" TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "UpdatedAt" TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "Status" VARCHAR(30) NOT NULL DEFAULT 'Submitted' CHECK ("Status" IN ('Submitted', 'Late', 'Graded', 'ResubmissionRequired')),
    "MarksObtained" INT NULL CHECK ("MarksObtained" IS NULL OR "MarksObtained" >= 0),
    "Feedback" TEXT NULL,
    "GradedAt" TIMESTAMPTZ NULL,
    "GradedByTeacherId" UUID NULL REFERENCES "Users"("Id") ON DELETE SET NULL,
    CONSTRAINT "UQ_Submissions_Assignment_Student" UNIQUE ("AssignmentId", "StudentId")
);

-- Indexes for optimal querying
CREATE INDEX "IX_Users_Email" ON "Users"("Email");
CREATE INDEX "IX_Subjects_ClassCourseId" ON "Subjects"("ClassCourseId");
CREATE INDEX "IX_TeacherSubjectAssignments_TeacherId" ON "TeacherSubjectAssignments"("TeacherId");
CREATE INDEX "IX_StudentEnrollments_StudentId" ON "StudentEnrollments"("StudentId");
CREATE INDEX "IX_Assignments_ClassCourseId" ON "Assignments"("ClassCourseId");
CREATE INDEX "IX_Assignments_TeacherId" ON "Assignments"("TeacherId");
CREATE INDEX "IX_Submissions_AssignmentId" ON "Submissions"("AssignmentId");
CREATE INDEX "IX_Submissions_StudentId" ON "Submissions"("StudentId");
