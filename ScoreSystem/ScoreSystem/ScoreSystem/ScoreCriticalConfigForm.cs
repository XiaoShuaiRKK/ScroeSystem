using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Org.BouncyCastle.Utilities.Collections;
using ScoreSystem.Data;
using ScoreSystem.Model;
using ScoreSystem.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScoreSystem
{
    public partial class ScoreCriticalConfigForm : Form
    {
        private FormAutoScaler autoScaler;
        private List<CriticalConfigDTO> previewConfigs = new List<CriticalConfigDTO>();
        private List<CriticalConfig> criticalConfigs;
        private List<CriticalConfig> editableConfigs; // 用于编辑模式下的副本
        private CriticalService criticalService = new CriticalService();
        private bool isEdited = false;

        public ScoreCriticalConfigForm()
        {
            InitializeComponent();
            autoScaler = new FormAutoScaler(this);
        }

        private void ScoreCriticalConfigForm_Load(object sender, EventArgs e)
        {
            this.Text = $"{ProjectSystemData.SYSTEM_NAME} - 临界配置";
            dataGridView_exams.ReadOnly = true;
            dataGridView_preview.ReadOnly = true;
            dataGridView_exams.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            InitPreviewGrid();
            CriticalConfigLoad();
        }

        private void InitPreviewGrid()
        {
            dataGridView_preview.DataSource = null;

            var bindingList = new BindingList<CriticalConfigDTO>(previewConfigs)
            {
                AllowNew = true,
                AllowEdit = true,
                AllowRemove = true
            };
            dataGridView_preview.DataSource = bindingList;

            // 先隐藏自动生成的 Grade、UniversityLevel、SubjectGroupId 等列（如果有）
            if (dataGridView_preview.Columns.Contains("Grade"))
                dataGridView_preview.Columns["Grade"].Visible = false;
            if (dataGridView_preview.Columns.Contains("UniversityLevel"))
                dataGridView_preview.Columns["UniversityLevel"].Visible = false;
            if (dataGridView_preview.Columns.Contains("SubjectGroupId"))
                dataGridView_preview.Columns["SubjectGroupId"].Visible = false;
            // 隐藏 Id 列
            if (dataGridView_preview.Columns.Contains("Id"))
                dataGridView_preview.Columns["Id"].Visible = false;


            // 添加年级下拉框列
            if (!dataGridView_preview.Columns.Contains("GradeCombo"))
            {
                var gradeList = Enum.GetValues(typeof(GradeEnum))
                                    .Cast<GradeEnum>()
                                    .Select(g => new { Name = g.ToString(), Value = (int)g })
                                    .ToList();
                var gradeCombo = new DataGridViewComboBoxColumn
                {
                    Name = "GradeCombo",
                    HeaderText = "年级",
                    DataPropertyName = "Grade",
                    DataSource = gradeList,
                    DisplayMember = "Name",
                    ValueMember = "Value",
                    FlatStyle = FlatStyle.Flat,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
                };
                dataGridView_preview.Columns.Insert(0, gradeCombo); // 放在最前面
            }

            // 添加大学等级下拉框列
            if (!dataGridView_preview.Columns.Contains("UniversityLevelCombo"))
            {
                var universityLevelList = Enum.GetValues(typeof(UniversityLevelEnum))
                                             .Cast<UniversityLevelEnum>()
                                             .Where(u => u != UniversityLevelEnum.全部)
                                             .Select(u => new { Name = u.ToString(), Value = (int)u })
                                             .ToList();
                var universityLevelCombo = new DataGridViewComboBoxColumn
                {
                    Name = "UniversityLevelCombo",
                    HeaderText = "大学等级",
                    DataPropertyName = "UniversityLevel",
                    DataSource = universityLevelList,
                    DisplayMember = "Name",
                    ValueMember = "Value",
                    FlatStyle = FlatStyle.Flat,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
                };
                // 放在年级后面
                dataGridView_preview.Columns.Insert(1, universityLevelCombo);
            }

            // 添加科目组合下拉框列
            if (!dataGridView_preview.Columns.Contains("SubjectGroupCombo"))
            {
                var subjectGroupList = Enum.GetValues(typeof(SubjectGroupEnum))
                                           .Cast<SubjectGroupEnum>()
                                           .Where(s => s != SubjectGroupEnum.未分组) // 过滤掉“未分组”
                                           .Select(s => new { Name = s.ToString(), Value = (long)s })
                                           .ToList();
                var subjectGroupCombo = new DataGridViewComboBoxColumn
                {
                    Name = "SubjectGroupCombo",
                    HeaderText = "科目组合",
                    DataPropertyName = "SubjectGroupId",
                    DataSource = subjectGroupList,
                    DisplayMember = "Name",
                    ValueMember = "Value",
                    ValueType = typeof(long),
                    FlatStyle = FlatStyle.Flat,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
                };
                // 放在大学等级后面
                dataGridView_preview.Columns.Insert(2, subjectGroupCombo);
            }

            // 设置其他列的标题
            if (dataGridView_preview.Columns.Contains("Year"))
                dataGridView_preview.Columns["Year"].HeaderText = "年份";
            if (dataGridView_preview.Columns.Contains("TargetCount"))
                dataGridView_preview.Columns["TargetCount"].HeaderText = "目标人数";
            if (dataGridView_preview.Columns.Contains("FloatUpCount"))
                dataGridView_preview.Columns["FloatUpCount"].HeaderText = "上浮人数";
            if (dataGridView_preview.Columns.Contains("FloatDownCount"))
                dataGridView_preview.Columns["FloatDownCount"].HeaderText = "下浮人数";

            EnsureDeleteButtonColumn();

            dataGridView_preview.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView_preview.AllowUserToAddRows = true;
            dataGridView_preview.AllowUserToDeleteRows = true;
            dataGridView_preview.ReadOnly = false;

            // 允许所有列可编辑
            foreach (DataGridViewColumn col in dataGridView_preview.Columns)
            {
                col.ReadOnly = false;
            }

            // 确保“操作”列在最后
            if (dataGridView_preview.Columns.Contains("DeleteLink"))
            {
                var deleteCol = dataGridView_preview.Columns["DeleteLink"];
                int lastIndex = dataGridView_preview.Columns.Count - 1;
                if (deleteCol.DisplayIndex != lastIndex)
                {
                    deleteCol.DisplayIndex = lastIndex;
                }
            }
        }

        private void EnsureDeleteButtonColumn()
        {
            if (!dataGridView_preview.Columns.Contains("DeleteLink"))
            {
                var deleteCol = new DataGridViewLinkColumn
                {
                    Name = "DeleteLink",
                    HeaderText = "操作",
                    Text = "删除",
                    UseColumnTextForLinkValue = true,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
                };
                dataGridView_preview.Columns.Add(deleteCol);
            }

            // 保证“操作”列在最后
            var deleteColumn = dataGridView_preview.Columns["DeleteLink"];
            if (deleteColumn != null)
            {
                deleteColumn.DisplayIndex = dataGridView_preview.Columns.Count - 1;
            }
        }


        private List<CriticalConfig> DeepClone(List<CriticalConfig> source)
        {
            return source.Select(c => new CriticalConfig
            {
                Id = c.Id,
                Grade = c.Grade,
                Year = c.Year,
                UniversityLevel = c.UniversityLevel,
                TargetCount = c.TargetCount,
                CriticalRatio = c.CriticalRatio,
                SubjectGroupId = c.SubjectGroupId,
                FloatUpCount = c.FloatUpCount,
                FloatDownCount = c.FloatDownCount
            }).ToList();
        }


        private async void CriticalConfigLoad()
        {
            try
            {
                this.criticalConfigs = await criticalService.GetCriticalConfigs();
                editableConfigs = DeepClone(criticalConfigs);

                dataGridView_exams.DataSource = null;
                dataGridView_exams.Columns.Clear();
                dataGridView_exams.AutoGenerateColumns = true;
                dataGridView_exams.DataSource = editableConfigs;

                CriticalConfigResetColumns(); // 设置列标题、隐藏无关列等

                // 添加“删除”操作列
                var deleteLink = new DataGridViewLinkColumn
                {
                    HeaderText = "操作",
                    Text = "删除",
                    UseColumnTextForLinkValue = true,
                    Name = "DeleteLink"
                };
                dataGridView_exams.Columns.Add(deleteLink);

                // 格式化枚举与比例显示
                dataGridView_exams.CellFormatting += (s, e) =>
                {
                    string colName = dataGridView_exams.Columns[e.ColumnIndex].Name;
                    if (colName == "Grade" && e.Value is int gradeVal)
                    {
                        e.Value = ((GradeEnum)gradeVal).ToString();
                        e.FormattingApplied = true;
                    }
                    else if (colName == "UniversityLevel" && e.Value is int levelVal)
                    {
                        e.Value = ((UniversityLevelEnum)levelVal).ToString();
                        e.FormattingApplied = true;
                    }
                    else if (colName == "SubjectGroupId" && e.Value is long groupVal)
                    {
                        e.Value = ((SubjectGroupEnum)groupVal).ToString();
                        e.FormattingApplied = true;
                    }
                    else if (colName == "CriticalRatio" && e.Value is double ratio)
                    {
                        e.Value = (ratio * 100).ToString("0.##") + "%";
                        e.FormattingApplied = true;
                    }
                };

                // 设置只读和选择模式
                dataGridView_exams.ReadOnly = true;
                dataGridView_exams.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dataGridView_exams.AllowUserToAddRows = false;

                // 注册点击事件
                dataGridView_exams.CellContentClick -= dataGridView_exams_CellContentClick; // 防止重复绑定
                dataGridView_exams.CellContentClick += dataGridView_exams_CellContentClick;
            }
            catch (Exception ex)
            {
                MessageBox.Show("加载配置失败: " + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void CriticalConfigResetColumns()
        {
            dataGridView_exams.Columns["Id"].HeaderText = "ID";
            dataGridView_exams.Columns["Grade"].HeaderText = "年级";
            dataGridView_exams.Columns["Year"].HeaderText = "年份";
            dataGridView_exams.Columns["UniversityLevel"].HeaderText = "大学等级";
            dataGridView_exams.Columns["TargetCount"].HeaderText = "目标人数";
            dataGridView_exams.Columns["CriticalRatio"].HeaderText = "临界比例";
            dataGridView_exams.Columns["SubjectGroupId"].HeaderText = "科目组合";
            dataGridView_exams.Columns["FloatUpCount"].HeaderText = "上浮人数";
            dataGridView_exams.Columns["FloatDownCount"].HeaderText = "下浮人数";

            foreach (DataGridViewColumn col in dataGridView_exams.Columns)
            {
                if (col.Name != "Id" && col.Name != "Grade" && col.Name != "Year" &&
                    col.Name != "UniversityLevel" && col.Name != "TargetCount" &&
                    col.Name != "CriticalRatio" && col.Name != "SubjectGroupId" &&
                    col.Name != "FloatUpCount" && col.Name != "FloatDownCount")
                {
                    col.Visible = false;
                }
            }
        }


        private void button_template_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
            saveFileDialog.FileName = "临界配置模板.xlsx";
            if (saveFileDialog.ShowDialog() != DialogResult.OK) return;

            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet("临界配置");

            // 提示说明行
            IRow tipRow = sheet.CreateRow(0);
            tipRow.HeightInPoints = 60;
            tipRow.CreateCell(0).SetCellValue("请按照本模板填写，删除或修改列顺序将导致导入失败。年级如“高一上学期”，组合如“理科”。大学等级只能为“九八五、双一流、优投、本科”此行禁止删除！！！");

            sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 0, 0, 6));
            IFont tipFont = workbook.CreateFont();
            tipFont.IsItalic = true;
            tipFont.Color = IndexedColors.Grey40Percent.Index;

            ICellStyle tipStyle = workbook.CreateCellStyle();
            tipStyle.SetFont(tipFont);
            tipStyle.WrapText = true;
            tipRow.GetCell(0).CellStyle = tipStyle;

            // 表头行
            IRow header = sheet.CreateRow(1);
            string[] headers = { "年级", "年份", "大学等级", "目标人数", "上浮人数", "下浮人数", "科目组合" };
            for (int i = 0; i < headers.Length; i++)
            {
                ICell cell = header.CreateCell(i);
                cell.SetCellValue(headers[i]);

                IFont boldFont = workbook.CreateFont();
                boldFont.IsBold = true;
                ICellStyle style = workbook.CreateCellStyle();
                style.SetFont(boldFont);
                cell.CellStyle = style;

                sheet.SetColumnWidth(i, 20 * 256); // 统一列宽
            }

            using (var fs = new FileStream(saveFileDialog.FileName, FileMode.Create, FileAccess.Write))
            {
                workbook.Write(fs);
            }

            MessageBox.Show("模板导出成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }


        private void button_import_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
            if (openFileDialog.ShowDialog() != DialogResult.OK) return;

            try
            {
                using (FileStream fs = new FileStream(openFileDialog.FileName, FileMode.Open, FileAccess.Read))
                {
                    IWorkbook workbook = new XSSFWorkbook(fs);
                    ISheet sheet = workbook.GetSheetAt(0);
                    previewConfigs.Clear();

                    for (int i = 2; i <= sheet.LastRowNum; i++)
                    {
                        IRow row = sheet.GetRow(i);
                        if (row == null || row.Cells.All(c => string.IsNullOrWhiteSpace(c?.ToString())))
                            continue;

                        string gradeStr = row.GetCell(0)?.ToString()?.Trim();
                        string yearStr = row.GetCell(1)?.ToString()?.Trim();
                        string levelStr = row.GetCell(2)?.ToString()?.Trim();
                        string targetStr = row.GetCell(3)?.ToString()?.Trim();
                        string floatUpStr = row.GetCell(4)?.ToString()?.Trim();
                        string floatDownStr = row.GetCell(5)?.ToString()?.Trim();
                        string groupStr = row.GetCell(6)?.ToString()?.Trim();

                        if (!IsValidEnumName<GradeEnum>(gradeStr, out var grade))
                            throw new Exception($"第{i + 1}行 年级解析失败或无效（只能输入对应名称,如高一上学期、高三下学期）：{gradeStr}");

                        if (!int.TryParse(yearStr, out var year))
                            throw new Exception($"第{i + 1}行 年份无效：{yearStr}");

                        if (!IsValidEnumName<UniversityLevelEnum>(levelStr, out var level))
                            throw new Exception($"第{i + 1}行 大学等级解析失败或无效（只能输入对应名称,九八五、双一流、优投、本科）：{levelStr}");

                        if (!int.TryParse(targetStr, out var targetCount) || targetCount < 1)
                            throw new Exception($"第{i + 1}行 目标人数无效：{targetStr}");

                        if (!int.TryParse(floatUpStr, out var floatUpCount) || floatUpCount < 0)
                            throw new Exception($"第{i + 1}行 上浮人数无效：{floatUpStr}");

                        if (!int.TryParse(floatDownStr, out var floatDownCount) || floatDownCount < 0)
                            throw new Exception($"第{i + 1}行 下浮人数无效：{floatDownStr}");

                        if (!IsValidEnumName<SubjectGroupEnum>(groupStr, out var group))
                            throw new Exception($"第{i + 1}行 科目组合解析失败或无效（只能输入对应名称，文科理科）：{groupStr}");

                        previewConfigs.Add(new CriticalConfigDTO
                        {
                            Grade = (int)grade,
                            Year = year,
                            UniversityLevel = (int)level,
                            TargetCount = targetCount,
                            FloatUpCount = floatUpCount,
                            FloatDownCount = floatDownCount,
                            SubjectGroupId = (int)group
                        });
                    }

                    InitPreviewGrid();
                    MessageBox.Show($"导入成功，共导入 {previewConfigs.Count} 条", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("导入失败：" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 严格验证字符串对应的枚举值是否有效，返回对应枚举值（out）
        /// </summary>
        private bool IsValidEnumName<TEnum>(string value, out TEnum enumValue) where TEnum : struct, Enum
        {
            enumValue = default;
            if (string.IsNullOrWhiteSpace(value))
                return false;

            if (Enum.TryParse<TEnum>(value, ignoreCase: true, out var parsedEnum))
            {
                var names = Enum.GetNames(typeof(TEnum));
                bool nameExists = names.Any(name => string.Equals(name, value, StringComparison.OrdinalIgnoreCase));
                if (nameExists)
                {
                    enumValue = parsedEnum;
                    return true;
                }
            }
            return false;
        }


        private async void button_save_Click(object sender, EventArgs e)
        {
            dataGridView_preview.EndEdit(); // 提交编辑中的值
            var list = (BindingList<CriticalConfigDTO>)dataGridView_preview.DataSource;
            if (!list.Any())
            {
                MessageBox.Show("没有可保存的数据", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var loading = new LoadForm())
            {
                button_save.Enabled = false;
                loading.Show();
                await Task.Delay(100);

                try
                {
                    bool success = await criticalService.AddScore(list.ToList());
                    if (success)
                    {
                        MessageBox.Show("保存成功", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        list.Clear();
                        CriticalConfigLoad(); // 重新加载左侧数据
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("保存异常: " + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    loading.Close();
                    button_save.Enabled = true;
                }
            }
        }

        private void dataGridView_preview_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dataGridView_preview.Columns[e.ColumnIndex].Name == "DeleteLink")
            {
                var bindingList = (BindingList<CriticalConfigDTO>)dataGridView_preview.DataSource;
                var item = dataGridView_preview.Rows[e.RowIndex].DataBoundItem as CriticalConfigDTO;
                if (item != null)
                {
                    bindingList.Remove(item);
                }
            }
        }

        private void dataGridView_preview_RowValidating(object sender, DataGridViewCellCancelEventArgs e)
        {
            try
            {
                var dgv = sender as DataGridView;
                if (e.RowIndex < 0 || e.RowIndex >= dgv.Rows.Count)
                    return;

                var row = dgv.Rows[e.RowIndex];

                if (row.IsNewRow || row.DataBoundItem == null)
                    return;

                if (!(row.DataBoundItem is CriticalConfigDTO config))
                    return;

                // 检查列存在，避免运行期异常
                if (!dgv.Columns.Contains("Grade") ||
                    !dgv.Columns.Contains("Year") ||
                    !dgv.Columns.Contains("UniversityLevel") ||
                    !dgv.Columns.Contains("TargetCount") ||
                    !dgv.Columns.Contains("SubjectGroupId") ||
                    !dgv.Columns.Contains("FloatUpCount") ||
                    !dgv.Columns.Contains("FloatDownCount"))
                    return;

                string errorMsg = "";

                // 声明并初始化变量，防止编译器报错
                int grade = 0;
                int year = 0;
                int level = 0;
                int targetCount = 0;
                long groupId = 0;
                int upCount = 0;
                int downCount = 0;

                // 安全访问单元格值
                object gradeObj = row.Cells["Grade"]?.Value;
                object yearObj = row.Cells["Year"]?.Value;
                object levelObj = row.Cells["UniversityLevel"]?.Value;
                object targetObj = row.Cells["TargetCount"]?.Value;
                object groupObj = row.Cells["SubjectGroupId"]?.Value;
                object floatUpObj = row.Cells["FloatUpCount"]?.Value;
                object floatDownObj = row.Cells["FloatDownCount"]?.Value;

                // 校验
                if (gradeObj == null || !int.TryParse(gradeObj.ToString(), out grade) || !Enum.IsDefined(typeof(GradeEnum), grade))
                    errorMsg += "年级无效；";

                if (yearObj == null || !int.TryParse(yearObj.ToString(), out year) || year < 2000 || year > DateTime.Now.Year + 10)
                    errorMsg += $"年份无效，应在 2000 ~ {DateTime.Now.Year + 10}；";

                if (levelObj == null || !int.TryParse(levelObj.ToString(), out level) || !Enum.IsDefined(typeof(UniversityLevelEnum), level))
                    errorMsg += "大学等级无效；";

                if (targetObj == null || !int.TryParse(targetObj.ToString(), out targetCount) || targetCount <= 0)
                    errorMsg += "目标人数应为大于0的整数；";

                if (groupObj == null || !long.TryParse(groupObj.ToString(), out groupId) || !Enum.IsDefined(typeof(SubjectGroupEnum), (SubjectGroupEnum)(int)groupId))
                    errorMsg += "科目组合无效；";

                if (floatUpObj == null || !int.TryParse(floatUpObj.ToString(), out upCount) || upCount < 0)
                    errorMsg += "上浮人数应为非负整数；";

                if (floatDownObj == null || !int.TryParse(floatDownObj.ToString(), out downCount) || downCount < 0)
                    errorMsg += "下浮人数应为非负整数；";

                if (!string.IsNullOrEmpty(errorMsg))
                {
                    MessageBox.Show($"第 {e.RowIndex + 1} 行验证失败：\n{errorMsg}", "验证失败", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    e.Cancel = true;
                    return;
                }

                // 验证 Grade + Year + UniversityLevel 的唯一性
                foreach (DataGridViewRow otherRow in dgv.Rows)
                {
                    if (otherRow.Index == e.RowIndex || otherRow.IsNewRow || otherRow.DataBoundItem == null)
                        continue;

                    if (otherRow.DataBoundItem is CriticalConfigDTO otherConfig)
                    {
                        if (otherConfig.Grade == grade &&
                            otherConfig.Year == year &&
                            otherConfig.UniversityLevel == level &&
                            otherConfig.SubjectGroupId == groupId)
                        {
                            MessageBox.Show($"第 {e.RowIndex + 1} 行验证失败：已存在相同的 年级+年份+大学等级 配置。", "验证失败", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            e.Cancel = true;
                            return;
                        }
                    }
                }

                // 更新对象属性
                config.Grade = grade;
                config.Year = year;
                config.UniversityLevel = level;
                config.TargetCount = targetCount;
                config.SubjectGroupId = groupId;
                config.FloatUpCount = upCount;
                config.FloatDownCount = downCount;
            }
            catch (Exception ex)
            {
                // 记录异常（可选）或忽略
                MessageBox.Show("验证异常：" + ex.Message);
                return;
            }
        }

        private void dataGridView_preview_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            //MessageBox.Show($"数据输入错误：{e.Exception.Message}", "数据错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            e.Cancel = true;
        }

        private void dataGridView_preview_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            e.Row.Cells["Grade"].Value = (int)GradeEnum.高一上学期;
            e.Row.Cells["Year"].Value = DateTime.Now.Year;
            e.Row.Cells["UniversityLevel"].Value = (int)UniversityLevelEnum.九八五;
            e.Row.Cells["TargetCount"].Value = 1;
            e.Row.Cells["FloatUpCount"].Value = 0;
            e.Row.Cells["FloatDownCount"].Value = 0;
            e.Row.Cells["SubjectGroupId"].Value = (int)SubjectGroupEnum.理科;
        }

        private async void dataGridView_exams_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dataGridView_exams.Columns[e.ColumnIndex].Name == "DeleteLink")
            {
                var confirm = MessageBox.Show("确认要删除该配置吗？", "删除确认", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (confirm == DialogResult.Yes)
                {
                    var row = dataGridView_exams.Rows[e.RowIndex];
                    if (row.DataBoundItem is CriticalConfig config)
                    {
                        try
                        {
                            await criticalService.DeleteCriticalConfig(config); // 调用服务删除
                            MessageBox.Show("删除成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            CriticalConfigLoad(); // 重新加载数据
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("删除失败: " + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
        }

        private async void button_edit_Click(object sender, EventArgs e)
        {
            if (!isEdited)
            {
                isEdited = true;
                button_edit.Text = "保存";
                dataGridView_exams.ReadOnly = false;
                dataGridView_exams.AllowUserToAddRows = false;
                dataGridView_exams.SelectionMode = DataGridViewSelectionMode.CellSelect;
                dataGridView_exams.Columns.Clear();
                dataGridView_exams.DataSource = null;
                dataGridView_exams.AutoGenerateColumns = false;

                // 深拷贝绑定，避免修改原数据
                editableConfigs = DeepClone(criticalConfigs);
                dataGridView_exams.DataSource = new BindingList<CriticalConfig>(editableConfigs);

                // 添加列
                dataGridView_exams.Columns.Add(new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "Id",
                    HeaderText = "ID",
                    Name = "Id",
                    ReadOnly = true
                });

                dataGridView_exams.Columns.Add(CreateEnumComboBoxColumn<GradeEnum>("Grade", "年级"));
                dataGridView_exams.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Year", HeaderText = "年份", Name = "Year" });
                dataGridView_exams.Columns.Add(CreateEnumComboBoxColumn<UniversityLevelEnum>("UniversityLevel", "大学等级", excludeValues: new[] { 0 }));
                dataGridView_exams.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "TargetCount", HeaderText = "目标人数", Name = "TargetCount" });

                dataGridView_exams.Columns.Add(new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "CriticalRatio",
                    HeaderText = "临界比例",
                    Name = "CriticalRatio",
                    ReadOnly = true
                });

                dataGridView_exams.Columns.Add(CreateEnumComboBoxColumn<SubjectGroupEnum>("SubjectGroupId", "科目组合", excludeValues: new[] { 1 }));
                dataGridView_exams.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "FloatUpCount", HeaderText = "上浮人数", Name = "FloatUpCount" });
                dataGridView_exams.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "FloatDownCount", HeaderText = "下浮人数", Name = "FloatDownCount" });
            }
            else
            {
                button_edit.Enabled = false;

                var updatedList = new List<CriticalConfig>();

                foreach (DataGridViewRow row in dataGridView_exams.Rows)
                {
                    if (row.IsNewRow) continue;

                    long id = Convert.ToInt64(row.Cells["Id"].Value);
                    var original = criticalConfigs.FirstOrDefault(c => c.Id == id);
                    var edited = editableConfigs.FirstOrDefault(c => c.Id == id);
                    if (original == null || edited == null) continue;

                    if (original.Grade != edited.Grade ||
                        original.Year != edited.Year ||
                        original.UniversityLevel != edited.UniversityLevel ||
                        original.TargetCount != edited.TargetCount ||
                        original.SubjectGroupId != edited.SubjectGroupId ||
                        original.FloatUpCount != edited.FloatUpCount ||
                        original.FloatDownCount != edited.FloatDownCount)
                    {
                        // 更新原始数据
                        original.Grade = edited.Grade;
                        original.Year = edited.Year;
                        original.UniversityLevel = edited.UniversityLevel;
                        original.TargetCount = edited.TargetCount;
                        original.SubjectGroupId = edited.SubjectGroupId;
                        original.FloatUpCount = edited.FloatUpCount;
                        original.FloatDownCount = edited.FloatDownCount;

                        updatedList.Add(original);
                    }
                }

                if (updatedList.Any())
                {
                    bool success = await criticalService.BatchUpdateCriticalConfigs(updatedList);
                    if (success)
                    {
                        MessageBox.Show("更新成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }

                isEdited = false;
                button_edit.Text = "修改";
                button_edit.Enabled = true;
                CriticalConfigLoad(); // 刷新数据
            }
        }


        private DataGridViewComboBoxColumn CreateEnumComboBoxColumn<TEnum>(string dataProperty, string headerText, int[] excludeValues = null)
            where TEnum : Enum
        {
            var values = Enum.GetValues(typeof(TEnum)).Cast<TEnum>();

            if (excludeValues != null)
            {
                values = values.Where(v => !excludeValues.Contains(Convert.ToInt32(v)));
            }

            var dataSource = values
                .Select(e => new { Value = Convert.ToInt32(e), Name = e.ToString() })
                .ToList();

            return new DataGridViewComboBoxColumn
            {
                Name = dataProperty,
                HeaderText = headerText,
                DataPropertyName = dataProperty,
                DataSource = dataSource,
                ValueMember = "Value",
                DisplayMember = "Name",
                ValueType = typeof(int),
                DisplayStyle = DataGridViewComboBoxDisplayStyle.DropDownButton
            };
        }



        private void dataGridView_exams_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.Cancel = true; // 静默忽略无效值异常
        }
    }
}
