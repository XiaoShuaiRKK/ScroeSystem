using System;
using System.Windows.Forms;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
using System.IO;
using System.Collections.Generic;
using ScoreSystem.Model;
using System.Linq;
using ScoreSystem.Data;
using NPOI.Util;
using ScoreSystem.Service;
using System.Threading.Tasks;

namespace ScoreSystem
{
    public partial class ScoreClassOrStudentOperateForm : Form
    {
        private FormAutoScaler autoScaler;
        private List<StudentDTO> students;
        private List<StudentFormVO> displayStudents;
        private StudentService studentService = StudentService.GetIntance();
        private ClassService classService = ClassService.GetIntance();
        public ScoreClassOrStudentOperateForm()
        {
            InitializeComponent();
            autoScaler = new FormAutoScaler(this);
        }
        private void ScoreClassOrStudentOperateForm_Load(object sender, EventArgs e)
        {
            this.Text = $"{ProjectSystemData.SYSTEM_NAME} - 学生导入";
            dataGridView_preview.ReadOnly = true;
            dataGridView_preview.MultiSelect = false;
            dataGridView_preview.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView_preview.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void button_template_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
                saveFileDialog.FileName = "学生信息模板.xlsx";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //创建一个Excel工程
                    IWorkbook workbook = new XSSFWorkbook();
                    ISheet sheet = workbook.CreateSheet("学生信息");

                    //提示
                    IRow tipRow = sheet.CreateRow(0);
                    tipRow.HeightInPoints = 60;
                    tipRow.CreateCell(0).SetCellValue("请按照本模板格式填写学生信息，严禁修改表头顺序。学生状态只能为正常、已毕业、劝退、休学。分科只能为文科、理科。选科不能重复不能为语数英物历。入学时间的格式应为2025-5-11，不能超过今天。此行不用删除！！！");
                    //合并单元格
                    sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 0, 0, 9));
                    // 设置提示样式
                    ICellStyle tipStyle = workbook.CreateCellStyle();
                    IFont tipFont = workbook.CreateFont();
                    tipFont.Color = IndexedColors.Grey40Percent.Index;
                    tipFont.IsItalic = true;
                    tipStyle.SetFont(tipFont);
                    tipStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Left;
                    tipStyle.WrapText = true;
                    tipRow.Cells[0].CellStyle = tipStyle;
                    //创建表头
                    IRow headerRow = sheet.CreateRow(1);
                    headerRow.CreateCell(0).SetCellValue("学号");
                    headerRow.CreateCell(1).SetCellValue("姓名");
                    headerRow.CreateCell(2).SetCellValue("用户名");
                    headerRow.CreateCell(3).SetCellValue("密码");
                    headerRow.CreateCell(4).SetCellValue("班级");
                    headerRow.CreateCell(5).SetCellValue("状态");
                    headerRow.CreateCell(6).SetCellValue("入学时间");
                    headerRow.CreateCell(7).SetCellValue("分科");
                    headerRow.CreateCell(8).SetCellValue("选科一");
                    headerRow.CreateCell(9).SetCellValue("选科二");
                    headerRow.CreateCell(10).SetCellValue("学年");
                    for (int i = 0; i <= 10; i++)
                    {
                        sheet.AutoSizeColumn(i);
                    }
                    using (FileStream fs = new FileStream(saveFileDialog.FileName, FileMode.Create, FileAccess.Write))
                    {
                        workbook.Write(fs);
                    }
                    MessageBox.Show("模板已保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("文档正在使用无法进行操作！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }

        private void button_import_student_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    List<StudentDTO> students = new List<StudentDTO>();
                    List<StudentFormVO> showStudents = new List<StudentFormVO>();
                    using (FileStream fs = new FileStream(openFileDialog.FileName, FileMode.Open, FileAccess.Read))
                    {
                        IWorkbook workbook = new XSSFWorkbook(fs);
                        ISheet sheet = workbook.GetSheetAt(0);

                        for (int i = 2; i <= sheet.LastRowNum; i++)
                        {
                            int displayRow = i + 1;
                            IRow row = sheet.GetRow(i);
                            if (row == null || row.Cells.All(c => c.CellType == CellType.Blank)) continue;

                            string studentNumber = row.GetCell(0)?.ToString().Trim();
                            string name = row.GetCell(1)?.ToString().Trim();
                            string username = row.GetCell(2)?.ToString().Trim();
                            string password = row.GetCell(3)?.ToString().Trim();
                            string className = row.GetCell(4)?.ToString().Trim();
                            string stateName = row.GetCell(5)?.ToString().Trim();
                            DateTime enrollmentDate = DataUtil.ParseDateCell(row.GetCell(6));
                            string subjectGroupName = row.GetCell(7)?.ToString().Trim();
                            string electiveCourse1Name = row.GetCell(8)?.ToString().Trim();
                            string electiveCourse2Name = row.GetCell(9)?.ToString().Trim();
                            string yearStr = row.GetCell(10)?.ToString().Trim();

                            // 状态校验
                            if (!DataUtil.TryParseStudentState(stateName, out StudentStateEnum state))
                            {
                                MessageBox.Show($"第{displayRow}行，第6列“状态”字段无效，只能是：正常、已毕业、劝退、休学。", "格式错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }

                            // 入学时间校验
                            if (enrollmentDate == DateTime.MinValue || enrollmentDate.Date > DateTime.Today)
                            {
                                MessageBox.Show($"第{displayRow}行，第7列“入学时间”字段无效，必须是有效且不晚于今天的日期。", "格式错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }

                            // 分科校验
                            if (!DataUtil.TryParseSubjectGroup(subjectGroupName, out SubjectGroupEnum subjectGroup))
                            {
                                MessageBox.Show($"第{displayRow}行，第8列“分科”字段无效，只能是：理科、文科。", "格式错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }

                            // 选科一校验
                            int course1 = DataUtil.ParseCouseNameToId(electiveCourse1Name);
                            if (electiveCourse1Name != "" && course1 == -1)
                            {
                                MessageBox.Show($"第{displayRow}行，第9列“选科一”字段无效。", "格式错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                            if (course1 >= 0 && course1 <= 4)
                            {
                                MessageBox.Show($"第{displayRow}行，第9列“选科一”只能为 化学,政治,地理,生物。", "格式错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }

                            // 选科二校验
                            int course2 = DataUtil.ParseCouseNameToId(electiveCourse2Name);
                            if (electiveCourse2Name != "" && course2 == -1)
                            {
                                MessageBox.Show($"第{displayRow}行，第10列“选科二”字段无效。", "格式错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                            if (course2 >= 0 && course2 <= 4)
                            {
                                MessageBox.Show($"第{displayRow}行，第10列“选科二”只能为 化学,政治,地理,生物。", "格式错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }

                            if (course1 == course2)
                            {
                                MessageBox.Show($"第{displayRow}行，两个选科不能重复。", "格式错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }

                            // 班级校验
                            int classId = classService.GetClassId(className);
                            if (classId == -1)
                            {
                                MessageBox.Show($"第{displayRow}行，第5列 班级格式错误或者班级不存在", "格式错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }

                            // 学年校验
                            if (!int.TryParse(yearStr, out int year) || year > DateTime.Now.Year || year < 1900)
                            {
                                MessageBox.Show($"第{displayRow}行，第11列“学年”字段无效，必须是1900-今年之间的整数。", "格式错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }

                            // 添加学生对象
                            StudentDTO student = new StudentDTO
                            {
                                StudentNumber = studentNumber,
                                Name = name,
                                UserName = username,
                                Password = password,
                                ClassId = classId,
                                State = (int)state,
                                EnrollmentDate = enrollmentDate.Date,
                                SubjectGroupId = (int)subjectGroup,
                                ElectiveCourse1Id = course1,
                                ElectiveCourse2Id = course2,
                                Year = year
                            };

                            students.Add(student);

                            // 展示对象
                            StudentFormVO studentForm = new StudentFormVO
                            {
                                StudentNumber = studentNumber,
                                Name = name,
                                UserName = username,
                                Password = password,
                                ClassName = className,
                                State = stateName,
                                EnrollmentDate = enrollmentDate.Date,
                                SubjectGroupName = subjectGroupName,
                                ElectiveCourse1Name = electiveCourse1Name,
                                ElectiveCourse2Name = electiveCourse2Name,
                                Year = year
                            };
                            showStudents.Add(studentForm);
                        }
                    }

                    this.students = students;
                    this.displayStudents = showStudents;
                    DataLoad();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("导入失败，请确认文件格式正确。\n" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void DataLoad()
        {
            dataGridView_preview.DataSource = null;
            dataGridView_preview.DataSource = displayStudents;
            // 设置列头显示名称（HeaderText）
            dataGridView_preview.Columns["StudentNumber"].HeaderText = "学号";
            dataGridView_preview.Columns["Name"].HeaderText = "姓名";
            dataGridView_preview.Columns["UserName"].HeaderText = "用户名";
            dataGridView_preview.Columns["Password"].HeaderText = "密码";
            dataGridView_preview.Columns["ClassName"].HeaderText = "班级";
            dataGridView_preview.Columns["State"].HeaderText = "状态";
            dataGridView_preview.Columns["EnrollmentDate"].HeaderText = "入学时间";
            dataGridView_preview.Columns["SubjectGroupName"].HeaderText = "分科";
            dataGridView_preview.Columns["ElectiveCourse1Name"].HeaderText = "选科一";
            dataGridView_preview.Columns["ElectiveCourse2Name"].HeaderText = "选科二";
            dataGridView_preview.Columns["Year"].HeaderText = "学年";
        }

        private void button_cancel_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private async void button_save_Click(object sender, EventArgs e)
        {
            using(var loading = new LoadForm())
            {
                loading.Show();
                await Task.Delay(100); // 确保窗口显示
                try
                {
                    if (students == null || !students.Any())
                    {
                        MessageBox.Show("未导入学生数据 无法添加", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    bool isSuccess = await studentService.AddStudent(students);
                    if (isSuccess)
                    {
                        MessageBox.Show("添加成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ClearData();
                    }
                }
                catch(Exception ex)
                {
                    MessageBox.Show($"保存失败: {ex.Message}");
                }
                finally
                {
                    loading.Close();
                }
            }
            
        }

        private void ClearData()
        {
            dataGridView_preview.DataSource = null;
            students = null;
            displayStudents = null;
        }
    }
}
