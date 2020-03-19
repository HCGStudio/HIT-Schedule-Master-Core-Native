using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace HCGStudio.HITScheduleMasterCore
{
    public struct NativeSchedule
    {
        public IntPtr Teacher;
        public IntPtr CourseName;
        public IntPtr Location;
        public int DayOfWeek;
        public int MaxWeek;
        public uint Week;
        public int CourseLength;
        public int CourseTime;
        public bool IsLongCourse;

        public void Free()
        {
            Marshal.FreeCoTaskMem(Teacher);
            Marshal.FreeCoTaskMem(CourseName);
            Marshal.FreeCoTaskMem(Location);
            Teacher = IntPtr.Zero;
            CourseName = IntPtr.Zero;
            Location = IntPtr.Zero;
        }
    }
}
