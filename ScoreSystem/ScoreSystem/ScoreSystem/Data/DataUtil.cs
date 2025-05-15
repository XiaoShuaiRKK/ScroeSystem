using NPOI.SS.UserModel;
using ScoreSystem.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoreSystem.Data
{
    public class DataUtil
    {
        public static int ParseIntCell(ICell cell)
        {
            if (cell == null) return 0;
            if (cell.CellType == CellType.Numeric) return (int)cell.NumericCellValue;
            if (int.TryParse(cell.ToString(), out int result)) return result;
            return 0;
        }

        public static DateTime ParseDateCell(ICell cell)
        {
            if (cell == null) throw new Exception("单元格为空，无法解析日期");

            // 如果是日期类型
            if (cell.CellType == CellType.Numeric && DateUtil.IsCellDateFormatted(cell))
            {
                return cell.DateCellValue.Date;
            }

            // 如果是字符串类型
            if (cell.CellType == CellType.String)
            {
                var str = cell.StringCellValue.Trim();
                if (DateTime.TryParse(str, out var result))
                {
                    return result.Date;
                }
            }
            return DateTime.MinValue;
        }

        public static int ParseCouseNameToId(string name)
        {
            if (Enum.TryParse<CourseEnum>(name.Trim(), out var result))
            {
                return (int)result;
            }
            else
            {
                throw new ArgumentException($"无法识别的课程名称: {name}");
            }
        }

        public static bool TryParseStudentState(string str, out StudentStateEnum state)
        {
            return Enum.TryParse<StudentStateEnum>(str, out state);
        }

        public static bool TryParseSubjectGroup(string str, out SubjectGroupEnum group)
        {
            return Enum.TryParse<SubjectGroupEnum>(str, out group);
        }

    }
}
