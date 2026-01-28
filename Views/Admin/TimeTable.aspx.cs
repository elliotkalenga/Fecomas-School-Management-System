using SMSWEBAPP.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SMSWEBAPP.Views.Admin
{
    public partial class TimeTable : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["User"] == null)
            {
                Response.Redirect("UserLogin.aspx");
            }

            if (!IsPostBack)
            {
                BindTimetable();
            }
        }

        private void BindTimetable()
        {
            var timetable = GenerateTimetable();
            DataTable dt = new DataTable();

            // Define columns: First column for Days, second for Class, remaining for Time Slots
            dt.Columns.Add("Weekday");
            dt.Columns.Add("Class");

            List<string> timeSlots = new List<string>
            {
                "07:30 - 08:10", "08:10 - 08:50", "08:50 - 09:30", "Health Break",
                "10:00 - 10:40", "10:40 - 11:20", "11:20 - 12:00", "Lunch Break",
                "13:00 - 13:40", "13:40 - 14:20", "14:20 - 15:00"
            };

            foreach (string slot in timeSlots)
            {
                dt.Columns.Add(slot);
            }

            // Fill the table with timetable data
            foreach (var day in timetable.Keys)
            {
                foreach (var classData in timetable[day])
                {
                    DataRow row = dt.NewRow();
                    row["Weekday"] = day;
                    row["Class"] = classData.Key; // Class name

                    // Ensure Health Break and Lunch Break are skipped
                    for (int i = 0; i < timeSlots.Count; i++)
                    {
                        string timeSlot = timeSlots[i];
                        if (timeSlot != "Health Break" && timeSlot != "Lunch Break")
                        {
                            var lesson = classData.Value.FirstOrDefault(x => x.Item1 == timeSlot);
                            row[timeSlot] = lesson != default ? lesson.Item2 + " (" + lesson.Item3 + ")" : "";
                        }
                    }

                    dt.Rows.Add(row);
                }
            }

            gvWeeklyTimetable.DataSource = dt;
            gvWeeklyTimetable.DataBind();
        }

        public DataTable GetSubjectAllocations()
        {
            DataTable dt = new DataTable();
            string query = "SELECT sa.ClassID, sa.TeacherID, sa.SubjectID, s.SubjectName, u.UserName AS TeacherName, c.ClassName " +
                           "FROM SubjectAllocation sa " +
                           "JOIN Subject s ON sa.SubjectID = s.SubjectID " +
                           "JOIN Users u ON sa.TeacherID = u.UserID " +
                           "JOIN Class c ON sa.ClassID = c.ClassID WHERE sa.SchoolId=@SchoolId";

            using (SqlConnection con = new SqlConnection(AppConnection.GetConnectionString()))
            {
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@SchoolId", Session["SchoolId"]);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }

        public Dictionary<string, Dictionary<string, List<(string, string, string)>>> GenerateTimetable()
        {
            DataTable allocations = GetSubjectAllocations();
            Dictionary<string, Dictionary<string, List<(string, string, string)>>> timetable =
                new Dictionary<string, Dictionary<string, List<(string, string, string)>>>(); // Day -> Class -> (TimeSlot, Subject, Teacher)

            List<string> weekdays = new List<string> { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday" };
            List<string> timeSlots = new List<string>
            {
                "07:30 - 08:10", "08:10 - 08:50", "08:50 - 09:30", "Health Break",
                "10:00 - 10:40", "10:40 - 11:20", "11:20 - 12:00", "Lunch Break",
                "13:00 - 13:40", "13:40 - 14:20", "14:20 - 15:00"
            };

            // Initialize timetable structure
            foreach (string day in weekdays)
            {
                timetable[day] = new Dictionary<string, List<(string, string, string)>>();
            }

            // Process subject allocations
            Dictionary<string, Queue<(string, string)>> classSubjectQueue = new Dictionary<string, Queue<(string, string)>>();

            foreach (DataRow row in allocations.Rows)
            {
                string className = row["ClassName"].ToString();
                string subject = row["SubjectName"].ToString();
                string teacher = row["TeacherName"].ToString();

                if (!classSubjectQueue.ContainsKey(className))
                {
                    classSubjectQueue[className] = new Queue<(string, string)>();
                }
                classSubjectQueue[className].Enqueue((subject, teacher));
            }

            // List of scheduled lessons to prevent teacher clashes
            Dictionary<string, HashSet<string>> scheduledTeachers = new Dictionary<string, HashSet<string>>();

            foreach (var day in weekdays)
            {
                scheduledTeachers[day] = new HashSet<string>(); // Keep track of teachers scheduled for the day
            }

            // Distribute subjects to time slots
            foreach (var className in classSubjectQueue.Keys)
            {
                int slotIndex = 0;

                foreach (var day in weekdays)
                {
                    if (!timetable[day].ContainsKey(className))
                    {
                        timetable[day][className] = new List<(string, string, string)>();
                    }

                    for (int i = 0; i < 9; i++) // Ensure there are 9 subjects
                    {
                        if (slotIndex >= timeSlots.Count) break;

                        // Skip Health Break and Lunch Break
                        if (timeSlots[slotIndex] == "Health Break" || timeSlots[slotIndex] == "Lunch Break")
                        {
                            slotIndex++;
                            continue;
                        }

                        if (classSubjectQueue[className].Count > 0)
                        {
                            var (currentSubject, currentTeacher) = classSubjectQueue[className].Dequeue();

                            // Ensure the teacher is not scheduled for another class in the same slot
                            if (!scheduledTeachers[day].Contains(currentTeacher))
                            {
                                timetable[day][className].Add((timeSlots[slotIndex], currentSubject, currentTeacher));
                                scheduledTeachers[day].Add(currentTeacher);
                            }
                            else
                            {
                                // Find another subject for this slot if possible
                                var alternativeSubjects = classSubjectQueue[className].ToList();
                                var availableSubject = alternativeSubjects.FirstOrDefault(s => !scheduledTeachers[day].Contains(s.Item2));

                                if (availableSubject != default)
                                {
                                    timetable[day][className].Add((timeSlots[slotIndex], availableSubject.Item1, availableSubject.Item2));
                                    scheduledTeachers[day].Add(availableSubject.Item2);
                                    classSubjectQueue[className].Dequeue(); // Remove from queue
                                }
                            }

                            classSubjectQueue[className].Enqueue((currentSubject, currentTeacher)); // Rotate subjects
                        }

                        slotIndex++;
                    }
                }
            }

            // Fill empty slots randomly until each class has 9 subjects per day
            foreach (var day in weekdays)
            {
                foreach (var className in timetable[day].Keys)
                {
                    var classSlots = timetable[day][className];
                    int emptySlots = 11 - classSlots.Count;

                    if (emptySlots > 0)
                    {
                        var subjectsQueue = new Queue<(string, string)>(classSubjectQueue[className]);

                        while (emptySlots > 0)
                        {
                            var subjectTeacher = subjectsQueue.Dequeue();
                            classSlots.Add((timeSlots[emptySlots - 1], subjectTeacher.Item1, subjectTeacher.Item2));
                            emptySlots--;
                            classSubjectQueue[className].Enqueue(subjectTeacher); // Re-enqueue the subject for rotation
                        }
                    }
                }
            }

            return timetable;
        }
    }
}
