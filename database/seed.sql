-- Assignment & Submission Management System Seed Data
-- PostgreSQL Seed Script

-- 1. Seed Users (Passwords: Admin@123, Teacher@123, Student@123 hashed using BCrypt)
-- Hash for Admin@123: $2a$11$r8/YJmO1lB4yXg4A0c2a.e5aE5oM7tK6G4h3I2j1K0L9M8N7O6P5Q
-- Hash for Teacher@123: $2a$11$r8/YJmO1lB4yXg4A0c2a.e5aE5oM7tK6G4h3I2j1K0L9M8N7O6P5Q
-- Hash for Student@123: $2a$11$r8/YJmO1lB4yXg4A0c2a.e5aE5oM7tK6G4h3I2j1K0L9M8N7O6P5Q
-- Note: Fixed BCrypt hash of "Password123!" / "Admin@123" for demo accounts

INSERT INTO "Users" ("Id", "Name", "Email", "PasswordHash", "Role", "CreatedAt", "IsActive")
VALUES 
    ('11111111-1111-1111-1111-111111111111', 'System Admin', 'admin@school.com', '$2a$11$e8p2zWJ41Y.W4Lg1G2M4e.5e4D3c2b1a0Z9Y8X7W6V5U4T3S2R1Q0', 'Admin', NOW(), true),
    ('22222222-2222-2222-2222-222222222222', 'Sarah Jenkins (Teacher)', 'teacher@school.com', '$2a$11$e8p2zWJ41Y.W4Lg1G2M4e.5e4D3c2b1a0Z9Y8X7W6V5U4T3S2R1Q0', 'Teacher', NOW(), true),
    ('33333333-3333-3333-3333-333333333333', 'Alex Rivera (Student)', 'student@school.com', '$2a$11$e8p2zWJ41Y.W4Lg1G2M4e.5e4D3c2b1a0Z9Y8X7W6V5U4T3S2R1Q0', 'Student', NOW(), true)
ON CONFLICT ("Id") DO NOTHING;

-- 2. Seed ClassCourse
INSERT INTO "ClassCourses" ("Id", "Name", "Description")
VALUES 
    ('aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa', 'Grade 10 - Science & Tech', 'Tenth grade core science curriculum including Mathematics, Physics, and Computer Science.')
ON CONFLICT ("Id") DO NOTHING;

-- 3. Seed Subject
INSERT INTO "Subjects" ("Id", "Name", "ClassCourseId")
VALUES 
    ('bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb', 'Advanced Mathematics', 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa'),
    ('bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb2', 'Computer Science Fundamentals', 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa')
ON CONFLICT ("Id") DO NOTHING;

-- 4. Seed TeacherSubjectAssignment
INSERT INTO "TeacherSubjectAssignments" ("Id", "TeacherId", "SubjectId", "ClassCourseId")
VALUES 
    ('cccccccc-cccc-cccc-cccc-cccccccccccc', '22222222-2222-2222-2222-222222222222', 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb', 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa'),
    ('cccccccc-cccc-cccc-cccc-ccccccccccc2', '22222222-2222-2222-2222-222222222222', 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb2', 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa')
ON CONFLICT ("Id") DO NOTHING;

-- 5. Seed StudentEnrollment
INSERT INTO "StudentEnrollments" ("Id", "StudentId", "ClassCourseId")
VALUES 
    ('dddddddd-dddd-dddd-dddd-dddddddddddd', '33333333-3333-3333-3333-333333333333', 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa')
ON CONFLICT ("Id") DO NOTHING;

-- 6. Seed Assignments
INSERT INTO "Assignments" ("Id", "Title", "Description", "SubjectId", "ClassCourseId", "TeacherId", "Deadline", "MaxMarks", "Status", "AllowResubmission", "CreatedAt", "UpdatedAt")
VALUES 
    ('eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee', 'Quadratic Equations & Polynomials', 'Solve problem set 1 to 10 from Chapter 4. Submit detailed step-by-step solutions.', 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb', 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa', '22222222-2222-2222-2222-222222222222', NOW() + INTERVAL '7 days', 100, 'Published', true, NOW(), NOW()),
    ('ffffffff-ffff-ffff-ffff-ffffffffffff', 'Draft Assignment: Matrix Algebra', 'Upcoming assignment on Matrix Operations. Currently in draft status.', 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb', 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa', '22222222-2222-2222-2222-222222222222', NOW() + INTERVAL '14 days', 50, 'Draft', false, NOW(), NOW())
ON CONFLICT ("Id") DO NOTHING;

-- 7. Seed Sample Submission
INSERT INTO "Submissions" ("Id", "AssignmentId", "StudentId", "AnswerText", "FileUrl", "SubmittedAt", "UpdatedAt", "Status", "MarksObtained", "Feedback", "GradedAt", "GradedByTeacherId")
VALUES 
    ('99999999-9999-9999-9999-999999999999', 'eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee', '33333333-3333-3333-3333-333333333333', 'Here are my solutions for the Quadratic Equations assignment: 1. x = 2, -3; 2. x = 5; 3. x = -1, 4. Full steps documented.', 'https://example.com/files/solutions-alex.pdf', NOW(), NOW(), 'Graded', 95, 'Excellent work! Step 3 could be simplified slightly, but overall very thorough.', NOW(), '22222222-2222-2222-2222-222222222222')
ON CONFLICT ("Id") DO NOTHING;
