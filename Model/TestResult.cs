using CommunityToolkit.Mvvm.ComponentModel;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uploadyahua.Model
{
    public partial class TestResult:ObservableObject
    {
         /// <summary>
        /// id
        /// </summary>
        [ObservableProperty]
        [property: SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int id;
        /// <summary>
        /// 样本编号
        /// </summary>
        [ObservableProperty]
        public string sampleNum;

        /// <summary>
        /// 样本类型
        /// </summary>
        [ObservableProperty]
        [property: SugarColumn(IsNullable = true)]
        public string sampleType;

        /// <summary>
        /// 患者姓名
        /// </summary>
        [ObservableProperty]
        [property: SugarColumn(IsNullable = true)]
        public string patientName;

        /// <summary>
        /// 患者编号/病历号
        /// </summary>
        [ObservableProperty]
        [property: SugarColumn(IsNullable = true)]
        public string patientNum;

        /// <summary>
        /// 年龄
        /// </summary>
        [ObservableProperty]
        [property: SugarColumn(IsNullable = true)]
        public string age;

        /// <summary>
        /// 性别
        /// </summary>
        [ObservableProperty]
        [property: SugarColumn(IsNullable = true)]
        public string gender;

        /// <summary>
        /// 出生日期
        /// </summary>
        [ObservableProperty]
        [property: SugarColumn(IsNullable = true)]
        public string birthday;

        /// <summary>
        /// 住院号
        /// </summary>
        [ObservableProperty]
        [property: SugarColumn(IsNullable = true)]
        public string admissionNum;

        /// <summary>
        /// 床号
        /// </summary>
        [ObservableProperty]
        [property: SugarColumn(IsNullable = true)]
        public string bedNum;

        /// <summary>
        /// 科室
        /// </summary>
        [ObservableProperty]
        [property: SugarColumn(IsNullable = true)]
        public string office;

        /// <summary>
        /// 医生姓名
        /// </summary>
        [ObservableProperty]
        [property: SugarColumn(IsNullable = true)]
        public string doctorName;

        /// <summary>
        /// 检验者
        /// </summary>
        [ObservableProperty]
        [property: SugarColumn(IsNullable = true)]
        public string proofer;

        /// <summary>
        /// 审核者
        /// </summary>
        [ObservableProperty]
        [property: SugarColumn(IsNullable = true)]
        public string auditor;

        /// <summary>
        /// 医院名字
        /// </summary>
        [ObservableProperty]
        [property: SugarColumn(IsNullable = true)]
        public string hospital;

        /// <summary>
        /// 就诊类型
        /// </summary>
        [ObservableProperty]
        [property: SugarColumn(IsNullable = true)]
        public string visitType;

        /// <summary>
        /// 送检日期
        /// </summary>
        [ObservableProperty]
        [property: SugarColumn(IsNullable = true)]
        public string submissionDate;

        /// <summary>
        /// 检测日期
        /// </summary>
        [ObservableProperty]
        [property: SugarColumn(IsNullable = true)]
        public string testDate;

        /// <summary>
        /// 备注
        /// </summary>
        [ObservableProperty]
        [property: SugarColumn(IsNullable = true)]
        public string remark;

        [ObservableProperty]
        [property:Navigate(NavigateType.OneToMany, nameof(Result.TestResultId))]
        public List<Result> result;
    }
}
