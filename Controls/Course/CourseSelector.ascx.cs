using System;
using Thinkgate.Classes;
using System.Linq;
using Telerik.Web.UI;

namespace Thinkgate.Controls.Course
{
    public partial class CourseSelector : TileControlBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                AddCourseMenuItems();
            }
        }

        private void AddCourseMenuItems()
        {
            var courses = Thinkgate.Base.Classes.Course.GetCourses();

            var grades = courses.Select(x => x.Grade.DisplayText).Distinct();

            foreach (string displayText in grades)
            {
                var gradeMenuItem = new RadMenuItem(displayText);
                coursesMenu.Items.Add(gradeMenuItem);

                var gradeSubjects = courses.Where(x => x.Grade.DisplayText == displayText).Select(x => x.Subject.DisplayText).Distinct();

                foreach (string subject in gradeSubjects)
                {
                    var subjectMenuItem = new RadMenuItem(subject);
                    gradeMenuItem.Items.Add(subjectMenuItem);

                    var subjectCourses = courses.Where(x => x.Grade.DisplayText == displayText 
                                                            && x.Subject.DisplayText == subject).Select(x => x.CourseName).Distinct();

                    foreach (string course in subjectCourses)
                    {
                        subjectMenuItem.Items.Add(new RadMenuItem(course));
                    }
                }
            }
        }
    }
}