Create Database Student_Management

Use Student_Management

Create Table Student
(StudentID int primary Key identity(1,1),
 SName varchar(20),
 Address varchar(50),
 DateOfBirth Date ,
 ContactInfo int
)

Create Table Enrollment
(
	EnrollmentID int primary Key identity(100,1),
	EnrollmentDate Date ,
	StudentID int References  Student
	)

Create Table Grade
(
	GradeID int primary Key identity(200,1),
	Score int,
	Remarks int,
	EvaluationDate Date,
	StudentID int References  Student
	)

	Create Table Assignment
	(
		AssignmentID int primary Key identity(300,1),
		DueDate Date,
		Title Varchar(50)
	)
	
	Create Table Assigned_Student
	(
		AssignmentID int References Assignment,
		StudentID int References Student
	)

	Create Table Submission
	(
	SubmissionID int primary Key identity(400,1),
	grade int,
	Feedback varchar(max),
	SubDate Date,
	StudentID int References  Student
	)


	Create Table Student_Submit
	(
	SubmissionID int References Submission ,
	StudentID int References  Student
	Primary key(SubmissionID,StudentID)
	)

	Create Table Course
	(
	CourseID int primary Key identity(600,1),
	CourseName varchar(50),
	Description Varchar(max)
	)

	Create Table Student_Course
	(
	CourseID int References Course ,
	StudentID int References Student
	Primary Key(CourseID,StudentID)
	)

	Create Table Teacher
	(
	TeacherID int primary Key identity(700,1),
	TeacherName varchar(50),
	ContactInfo int,
	Specialization Varchar(max)
	)

	Create Table Teacher_course
	(
		CourseID int References Course,
		TeacherID int References Teacher
		primary key(CourseID,TeacherID)
	)

	INSERT INTO Student (SName, Address, DateOfBirth, ContactInfo) VALUES
('Student1','Cairo','2000-01-01',1000000001),
('Student2','Giza','2000-02-02',1000000002),
('Student3','Alex','2000-03-03',1000000003),
('Student4','Tanta','2000-04-04',1000000004),
('Student5','Mansoura','2000-05-05',1000000005),
('Student6','Cairo','2000-06-06',1000000006),
('Student7','Giza','2000-07-07',1000000007),
('Student8','Alex','2000-08-08',1000000008),
('Student9','Tanta','2000-09-09',1000000009),
('Student10','Mansoura','2000-10-10',1000000010),
('Student11','Cairo','2001-01-01',1000000011),
('Student12','Giza','2001-02-02',1000000012),
('Student13','Alex','2001-03-03',1000000013),
('Student14','Tanta','2001-04-04',1000000014),
('Student15','Mansoura','2001-05-05',1000000015),
('Student16','Cairo','2001-06-06',1000000016),
('Student17','Giza','2001-07-07',1000000017),
('Student18','Alex','2001-08-08',1000000018),
('Student19','Tanta','2001-09-09',1000000019),
('Student20','Mansoura','2001-10-10',1000000020),
('Student21','Cairo','2002-01-01',1000000021),
('Student22','Giza','2002-02-02',1000000022),
('Student23','Alex','2002-03-03',1000000023),
('Student24','Tanta','2002-04-04',1000000024),
('Student25','Mansoura','2002-05-05',1000000025),
('Student26','Cairo','2002-06-06',1000000026),
('Student27','Giza','2002-07-07',1000000027),
('Student28','Alex','2002-08-08',1000000028),
('Student29','Tanta','2002-09-09',1000000029),
('Student30','Mansoura','2002-10-10',1000000030),
('Student31','Cairo','2003-01-01',1000000031),
('Student32','Giza','2003-02-02',1000000032),
('Student33','Alex','2003-03-03',1000000033),
('Student34','Tanta','2003-04-04',1000000034),
('Student35','Mansoura','2003-05-05',1000000035),
('Student36','Cairo','2003-06-06',1000000036),
('Student37','Giza','2003-07-07',1000000037),
('Student38','Alex','2003-08-08',1000000038),
('Student39','Tanta','2003-09-09',1000000039),
('Student40','Mansoura','2003-10-10',1000000040),
('Student41','Cairo','2004-01-01',1000000041),
('Student42','Giza','2004-02-02',1000000042),
('Student43','Alex','2004-03-03',1000000043),
('Student44','Tanta','2004-04-04',1000000044),
('Student45','Mansoura','2004-05-05',1000000045),
('Student46','Cairo','2004-06-06',1000000046),
('Student47','Giza','2004-07-07',1000000047),
('Student48','Alex','2004-08-08',1000000048),
('Student49','Tanta','2004-09-09',1000000049),
('Student50','Mansoura','2004-10-10',1000000050);

INSERT INTO Course (CourseName, Description) VALUES
('Database','SQL Server'),
('OOP','Object Oriented Programming'),
('Algorithms','Data Structures'),
('Web','ASP.NET Core'),
('AI','Artificial Intelligence');

INSERT INTO Teacher (TeacherName, ContactInfo, Specialization) VALUES
('Dr Ahmed',1111111111,'Database'),
('Dr Mona',2222222222,'OOP'),
('Dr Kareem',3333333333,'Algorithms'),
('Dr Huda',4444444444,'Web'),
('Dr Samy',5555555555,'AI');



INSERT INTO Enrollment (EnrollmentDate,StudentID) VALUES
('2025-01-01',1),('2025-01-01',2),('2025-01-01',3),('2025-01-01',4),('2025-01-01',5),
('2025-01-02',6),('2025-01-02',7),('2025-01-02',8),('2025-01-02',9),('2025-01-02',10),
('2025-01-03',11),('2025-01-03',12),('2025-01-03',13),('2025-01-03',14),('2025-01-03',15),
('2025-01-04',16),('2025-01-04',17),('2025-01-04',18),('2025-01-04',19),('2025-01-04',20),
('2025-01-05',21),('2025-01-05',22),('2025-01-05',23),('2025-01-05',24),('2025-01-05',25),
('2025-01-06',26),('2025-01-06',27),('2025-01-06',28),('2025-01-06',29),('2025-01-06',30),
('2025-01-07',31),('2025-01-07',32),('2025-01-07',33),('2025-01-07',34),('2025-01-07',35),
('2025-01-08',36),('2025-01-08',37),('2025-01-08',38),('2025-01-08',39),('2025-01-08',40),
('2025-01-09',41),('2025-01-09',42),('2025-01-09',43),('2025-01-09',44),('2025-01-09',45),
('2025-01-10',46),('2025-01-10',47),('2025-01-10',48),('2025-01-10',49),('2025-01-10',50);

INSERT INTO Assignment (DueDate,Title) VALUES
('2025-06-01','Database Assignment 1'),
('2025-06-15','Database Assignment 2'),
('2025-07-01','OOP Assignment 1'),
('2025-07-15','OOP Assignment 2'),
('2025-08-01','Algorithms Assignment'),
('2025-08-10','Web Assignment'),
('2025-08-20','AI Assignment');

INSERT INTO Grade (Score,Remarks,EvaluationDate,StudentID) VALUES
(90,1,'2025-06-10',1),
(75,1,'2025-06-10',2),
(88,1,'2025-06-10',3),
(60,0,'2025-06-10',4),
(95,1,'2025-06-10',5),
(70,1,'2025-06-10',6),
(82,1,'2025-06-10',7),
(66,0,'2025-06-10',8),
(91,1,'2025-06-10',9),
(73,1,'2025-06-10',10),
(85,1,'2025-06-10',11),
(77,1,'2025-06-10',12),
(69,0,'2025-06-10',13),
(92,1,'2025-06-10',14),
(88,1,'2025-06-10',15),
(74,1,'2025-06-10',16),
(80,1,'2025-06-10',17),
(63,0,'2025-06-10',18),
(97,1,'2025-06-10',19),
(71,1,'2025-06-10',20);

INSERT INTO Submission (grade,Feedback,SubDate,StudentID) VALUES
(85,'Good','2025-05-20',1),
(70,'Average','2025-05-20',2),
(90,'Excellent','2025-05-20',3),
(60,'Needs Work','2025-05-20',4),
(95,'Excellent','2025-05-20',5),
(72,'Good','2025-05-21',6),
(88,'Very Good','2025-05-21',7),
(66,'Average','2025-05-21',8),
(91,'Excellent','2025-05-21',9),
(73,'Good','2025-05-21',10);
INSERT INTO Student_Course VALUES
(600,1),(600,2),(600,3),(600,4),(600,5),
(601,6),(601,7),(601,8),(601,9),(601,10),
(602,11),(602,12),(602,13),(602,14),(602,15),
(603,16),(603,17),(603,18),(603,19),(603,20),
(604,21),(604,22),(604,23),(604,24),(604,25);
INSERT INTO Student_Submit VALUES
(400,1),
(401,2),
(402,3),
(403,4),
(404,5);
INSERT INTO Assigned_Student VALUES
(300,1),(300,2),(300,3),(300,4),(300,5),
(301,6),(301,7),(301,8),(301,9),(301,10),
(302,11),(302,12),(302,13),(302,14),(302,15),
(303,16),(303,17),(303,18),(303,19),(303,20),
(304,21),(304,22),(304,23),(304,24),(304,25);