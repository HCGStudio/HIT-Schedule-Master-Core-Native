using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Ical.Net.Serialization;
using Newtonsoft.Json;

namespace HCGStudio.HITScheduleMasterCore
{
    public static class NativeMethods
    {
        private static Schedule schedule;

        [NativeCallable(EntryPoint = "SetEmptySchedule")]
        public static bool SetEmpty(int year, int semester)
        {
            try
            {
                schedule = new Schedule(year, (Semester)semester);
            }
            catch
            {
                return true;
            }

            return true;
        }

        [NativeCallable(EntryPoint = "ReadScheduleFromFile")]
        public static bool ReadFromXlsFile(string fileName)
        {
            try
            {
                schedule = Schedule.LoadFromStream(File.Open(fileName, FileMode.Open));
            }
            catch
            {
                return false;
            }

            return true;
        }

        [NativeCallable(EntryPoint = "WriteScheduleToIcalFile")]
        public static bool SaveToFile(string fileName)
        {
            try
            {
                File.WriteAllText(fileName,
                    new CalendarSerializer().SerializeToString(schedule.GetCalendar()),
                    new UTF8Encoding(false));
            }
            catch
            {
                return false;
            }

            return true;
        }

        [NativeCallable(EntryPoint = "ConvertScheduleToJson")]
        public static IntPtr ConvertToJson()
        {
            return Marshal.StringToCoTaskMemAnsi(JsonConvert.SerializeObject(schedule));
        }

        [NativeCallable(EntryPoint = "FreeClrUnmanagedPtr")]
        public static void FreeClrUnmanagedPtr(IntPtr ptr)
        {
            Marshal.FreeCoTaskMem(ptr);
        }

        [NativeCallable(EntryPoint = "GetNativeScheduleEntries")]
        public static unsafe IntPtr GetNativeScheduleEntries()
        {
            var returnPtr = Marshal.AllocCoTaskMem(schedule.Count * sizeof(NativeSchedule));
            var arr = (NativeSchedule*)returnPtr.ToPointer();
            for (var i = 0; i < schedule.Count; ++i)
            {
                arr[i] = schedule[i].ToNativeSchedule();
            }
            return returnPtr;
        }

        [NativeCallable(EntryPoint = "FreeGotScheduleEntries")]
        public static unsafe void FreeGotScheduleEntries(IntPtr ptr)
        {
            var length = Marshal.SizeOf(ptr) / sizeof(NativeSchedule);
            var arr = (NativeSchedule*)ptr.ToPointer();
            for (var i = 0; i < length; ++i)
            {
                arr[i].Free();
            }
            Marshal.FreeCoTaskMem(ptr);
        }

        [NativeCallable(EntryPoint = "ImportScheduleFromJson")]
        public static void ImportFromJson(string json)
        {
            schedule = JsonConvert.DeserializeObject<Schedule>(json);
        }

    }
}
