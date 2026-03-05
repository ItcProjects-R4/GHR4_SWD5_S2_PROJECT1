------------------------------------------------------------------------------------------
-- Aggregation Functions to Improve Database System Quality of Life
------------------------------------------------------------------------------------------

-- A query to retrieve the total number of students, courses, and teachers in the database:
SELECT 
    (SELECT COUNT(*) FROM Student) AS Total_Students,
    (SELECT COUNT(*) FROM Course) AS Total_Courses,
    (SELECT COUNT(*) FROM Teacher) AS Total_Teachers


-- A query to retrieve Course Enrollment & Popularity
SELECT 
    c.CourseName, 
    COUNT(sc.StudentID) AS Enrolled_Students
FROM Course c
LEFT JOIN Student_Course sc ON c.CourseID = sc.CourseID
GROUP BY c.CourseName;


-- A query to retrieve how many students are in each course
SELECT CourseID, COUNT(StudentID) AS Enrolled_Students
FROM Student_Course
GROUP BY CourseID;


-- A query to retrieve the highest and lowest grades in the Grade table
SELECT MAX(Score) AS Top_Score, MIN(Score) AS Lowest_Score
FROM Grade;

-- A query to retrieve the average grade on all turned-in assignments
SELECT AVG(grade) AS Avg_Submission_Grade
FROM Submission;

-- A query to retrieve each student's average GPA across all their evaluations 
SELECT StudentID, AVG(Score) AS GPA
FROM Grade
GROUP BY StudentID
ORDER BY GPA DESC;

-- A query to retrieve the avergage number of students that passed (score>=50)
SELECT COUNT(StudentID) AS Students_Passed
FROM Grade
WHERE Score >= 50;