namespace PathwayPlatform.Models
{
    public static class DataSeeder
    {
        public static void Seed(AppDbContext db)
        {
            SeedUsers(db);
            SeedStudentProfiles(db);
            SeedInitiatives(db);
            SeedOpportunities(db);
        }

        // ── 1. Users ──────────────────────────────────────────────────────────
        private static void SeedUsers(AppDbContext db)
        {
            if (db.Users.Any()) return;

            var now = DateTime.UtcNow;
            db.Users.AddRange(
                new User { Name = "Admin",        Email = "admin@pathway.dev",       Password = "Admin@123",   Role = UserRole.Admin,       CreatedAt = now },
                new User { Name = "Ahmed Hassan", Email = "ahmed@student.dev",        Password = "Student@123", Role = UserRole.Student,     CreatedAt = now.AddDays(-30) },
                new User { Name = "Sara Khalil",  Email = "sara@student.dev",         Password = "Student@123", Role = UserRole.Student,     CreatedAt = now.AddDays(-25) },
                new User { Name = "Cairo Tech",   Email = "cairo.tech@contrib.dev",   Password = "Contrib@123", Role = UserRole.Contributor, CreatedAt = now.AddDays(-20) }
            );
            db.SaveChanges();
        }

        // ── 2. Student Profiles ───────────────────────────────────────────────
        private static void SeedStudentProfiles(AppDbContext db)
        {
            if (db.StudentProfiles.Any()) return;

            var ahmed = db.Users.FirstOrDefault(u => u.Email == "ahmed@student.dev");
            var sara  = db.Users.FirstOrDefault(u => u.Email == "sara@student.dev");
            if (ahmed is null && sara is null) return;

            var profiles = new List<StudentProfile>();
            var now = DateTime.UtcNow;

            if (ahmed is not null)
            {
                profiles.Add(new StudentProfile
                {
                    UserId       = ahmed.Id,
                    FullName     = "Ahmed Hassan Mohamed",
                    Bio          = "Passionate computer science student with a love for backend development " +
                                   "and open-source projects. Currently focused on ASP.NET Core and cloud technologies.",
                    PhoneNumber  = "+20 100 000 1234",
                    Address      = "Cairo, Egypt",
                    DateOfBirth  = new DateTime(2001, 6, 15),
                    Gender       = "Male",
                    University   = "Cairo University",
                    Faculty      = "Faculty of Computers and Artificial Intelligence",
                    Major        = "Computer Science",
                    AcademicYear = "3rd Year",
                    Level        = "Junior",
                    GPA          = 3.7,
                    Skills       = "C#,ASP.NET Core,SQL Server,Entity Framework,JavaScript,Git",
                    LinkedInUrl  = "https://linkedin.com/in/ahmedhassan",
                    GithubUrl    = "https://github.com/ahmedhassan",
                    PortfolioUrl = "https://ahmedhassan.dev",
                    CreatedAt    = now.AddDays(-30),
                    UpdatedAt    = now.AddDays(-5)
                });
            }

            if (sara is not null)
            {
                profiles.Add(new StudentProfile
                {
                    UserId       = sara.Id,
                    FullName     = "Sara Khalil Ahmed",
                    Bio          = "Data science enthusiast and aspiring ML engineer. " +
                                   "I enjoy turning messy datasets into clear insights.",
                    PhoneNumber  = "+20 110 000 5678",
                    Address      = "Alexandria, Egypt",
                    DateOfBirth  = new DateTime(2002, 3, 22),
                    Gender       = "Female",
                    University   = "Ain Shams University",
                    Faculty      = "Faculty of Engineering",
                    Major        = "Information Systems",
                    AcademicYear = "2nd Year",
                    Level        = "Sophomore",
                    GPA          = 3.5,
                    Skills       = "Python,Pandas,NumPy,scikit-learn,SQL,Tableau",
                    LinkedInUrl  = "https://linkedin.com/in/sarakhalil",
                    GithubUrl    = "https://github.com/sarakhalil",
                    CreatedAt    = now.AddDays(-25),
                    UpdatedAt    = now.AddDays(-3)
                });
            }

            if (profiles.Any())
            {
                db.StudentProfiles.AddRange(profiles);
                db.SaveChanges();
            }
        }

        // ── 3. Initiatives / Tracks / Courses ─────────────────────────────────
        private static void SeedInitiatives(AppDbContext db)
        {
            if (db.Initiatives.Any()) return;

            var initiatives = new List<Initiative>
            {
                new Initiative
                {
                    Name        = "ITI Open Source Track",
                    Description = "Information Technology Institute's flagship open-source learning program.",
                    Tracks = new List<Track>
                    {
                        new Track
                        {
                            Name = "Web Development", Type = "Technical",
                            Description = "Full-stack web development with modern frameworks.",
                            Courses = new List<Course>
                            {
                                new Course { Title = "HTML & CSS Fundamentals", Description = "Build solid foundations for the web.",  Duration = "3 weeks", SourceType = "Video",    ContentUrl = "https://developer.mozilla.org/en-US/docs/Learn", CreatedAt = DateTime.UtcNow },
                                new Course { Title = "JavaScript Essentials",   Description = "Core JS concepts and the DOM.",         Duration = "4 weeks", SourceType = "Video",    ContentUrl = "https://javascript.info",                        CreatedAt = DateTime.UtcNow },
                                new Course { Title = "ASP.NET Core MVC",        Description = "Build server-side web apps.",           Duration = "6 weeks", SourceType = "Tutorial", ContentUrl = "https://learn.microsoft.com/aspnet/core",        CreatedAt = DateTime.UtcNow },
                            }
                        },
                        new Track
                        {
                            Name = "Data Science", Type = "Technical",
                            Description = "Data analysis, machine learning, and visualization.",
                            Courses = new List<Course>
                            {
                                new Course { Title = "Python for Data Science", Description = "Python fundamentals for data work.", Duration = "4 weeks", SourceType = "Video", ContentUrl = "https://www.python.org/about/gettingstarted/", CreatedAt = DateTime.UtcNow },
                                new Course { Title = "Pandas & NumPy",          Description = "Data wrangling essentials.",         Duration = "3 weeks", SourceType = "Video", ContentUrl = "https://pandas.pydata.org/docs/",              CreatedAt = DateTime.UtcNow },
                            }
                        }
                    }
                },
                new Initiative
                {
                    Name        = "Google Digital Skills",
                    Description = "Google's curated digital skills program for career readiness.",
                    Tracks = new List<Track>
                    {
                        new Track
                        {
                            Name = "Digital Marketing", Type = "Marketing",
                            Description = "SEO, SEM, social media, and analytics.",
                            Courses = new List<Course>
                            {
                                new Course { Title = "SEO Fundamentals",   Description = "Search engine optimization basics.", Duration = "2 weeks", SourceType = "Article", ContentUrl = "https://developers.google.com/search/docs", CreatedAt = DateTime.UtcNow },
                                new Course { Title = "Google Analytics 4", Description = "Measure and analyze web traffic.",   Duration = "2 weeks", SourceType = "Video",   ContentUrl = "https://skillshop.withgoogle.com",         CreatedAt = DateTime.UtcNow },
                            }
                        },
                        new Track
                        {
                            Name = "Cloud Computing", Type = "Technical",
                            Description = "Google Cloud Platform foundations.",
                            Courses = new List<Course>
                            {
                                new Course { Title = "GCP Fundamentals",   Description = "Core GCP services.",         Duration = "4 weeks", SourceType = "Video", ContentUrl = "https://cloud.google.com/training",     CreatedAt = DateTime.UtcNow },
                                new Course { Title = "Cloud Architecture",  Description = "Designing cloud solutions.", Duration = "5 weeks", SourceType = "Video", ContentUrl = "https://cloud.google.com/architecture", CreatedAt = DateTime.UtcNow },
                            }
                        }
                    }
                },
                new Initiative
                {
                    Name        = "Udacity Nanodegrees",
                    Description = "Industry-certified nanodegree programs across tech disciplines.",
                    Tracks = new List<Track>
                    {
                        new Track
                        {
                            Name = "AI & Machine Learning", Type = "Technical",
                            Description = "Deep learning, NLP, and computer vision.",
                            Courses = new List<Course>
                            {
                                new Course { Title = "Intro to Machine Learning", Description = "Supervised and unsupervised learning.", Duration = "6 weeks", SourceType = "Video", ContentUrl = "https://www.udacity.com/course/intro-to-machine-learning--ud120", CreatedAt = DateTime.UtcNow },
                                new Course { Title = "Deep Learning Foundations", Description = "Neural networks with PyTorch.",          Duration = "8 weeks", SourceType = "Video", ContentUrl = "https://www.udacity.com/course/deep-learning-pytorch--ud188",    CreatedAt = DateTime.UtcNow },
                            }
                        }
                    }
                }
            };

            db.Initiatives.AddRange(initiatives);
            db.SaveChanges();
        }

        // ── 4. Opportunities ──────────────────────────────────────────────────
        private static void SeedOpportunities(AppDbContext db)
        {
            if (db.Opportunities.Any()) return;

            db.Opportunities.AddRange(
                new Opportunity { Title = "Junior Frontend Developer",   Type = "Job",        Source = "LinkedIn",    RequiredSkills = "HTML,CSS,JavaScript",  Link = "https://linkedin.com",     CreatedAt = DateTime.UtcNow, Description = "Build responsive UIs for a SaaS product." },
                new Opportunity { Title = "Data Science Internship",      Type = "Internship", Source = "Glassdoor",   RequiredSkills = "Python,Pandas,SQL",     Link = "https://glassdoor.com",    CreatedAt = DateTime.UtcNow, Description = "3-month paid data science internship." },
                new Opportunity { Title = "AWS Cloud Practitioner Event", Type = "Event",      Source = "Eventbrite",  RequiredSkills = "Cloud Basics",          Link = "https://eventbrite.com",   CreatedAt = DateTime.UtcNow, Description = "Free online certification prep event." },
                new Opportunity { Title = "Backend .NET Developer",       Type = "Job",        Source = "Wuzzuf",      RequiredSkills = "C#,ASP.NET,SQL Server", Link = "https://wuzzuf.net",       CreatedAt = DateTime.UtcNow, Description = "Work on enterprise backend systems." },
                new Opportunity { Title = "Digital Marketing Graduate",   Type = "Internship", Source = "LinkedIn",    RequiredSkills = "SEO,Content,Analytics", Link = "https://linkedin.com",     CreatedAt = DateTime.UtcNow, Description = "Support a marketing team for 6 months." },
                new Opportunity { Title = "Google I/O Extended Egypt",    Type = "Event",      Source = "GDG",         RequiredSkills = "Android,Web,AI",        Link = "https://gdg.community.dev",CreatedAt = DateTime.UtcNow, Description = "Attend Google I/O sessions locally." }
            );
            db.SaveChanges();
        }
    }
}
