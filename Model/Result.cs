using CommunityToolkit.Mvvm.ComponentModel;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uploadyahua.Model
{
    public partial class Result:ObservableObject
    {
        /// <summary>
        /// id
        /// </summary>
        [ObservableProperty]
        [property: SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int id;
        /// <summary>
        /// id2
        /// </summary>
        [ObservableProperty]
        public int testResultId;
        /// <summary>
        /// 流水号
        /// </summary>
        [ObservableProperty]
        [property: SugarColumn(IsNullable = true)]
        public string serialNum;
         /// <summary>
        /// 检测编号
        /// </summary>
        [ObservableProperty]
        [property: SugarColumn(IsNullable = true)]
        public string testNum;
         /// <summary>
        /// 检测项目
        /// </summary>
        [ObservableProperty]
        [property: SugarColumn(IsNullable = true)]
        public string testItem;
         /// <summary>
        /// 检测值
        /// </summary>
        [ObservableProperty]
        [property: SugarColumn(IsNullable = true)]
        public string testValue;
         /// <summary>
        /// 检测结果
        /// </summary>
        [ObservableProperty]
        [property: SugarColumn(IsNullable = true)]
        public string testResult;
         /// <summary>
        /// 参考值
        /// </summary>
        [ObservableProperty]
        [property: SugarColumn(IsNullable = true)]
        public string reference;
    }
}
