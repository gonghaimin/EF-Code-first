using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EF_Code_first
{
    /// <summary>
    /// 一对多的关系
    /// </summary>
    public class Grade
    {
        [Key]
        public int Id { get; set; }
        public string GradeName { get; set; }
        public List<@Class> ClassList { get; set; }
    }
    public class @Class
    {
        [Key]
        public int Id { get; set; }
        public string ClassName { get; set; }
        public int StudentCount { get; set; }
        /// <summary>
        /// Code First就能通过一些引用属性、导航属性等检测到模型之间的关系，自动为我们生成外键。
        /// 我们也可以使用特性ForeignKey指定一个外键，但是要注意，name值必须在导航属性中存在。
        /// 本示例中Student类必须有一个Class_Id的属性。
        /// </summary>
        [ForeignKey("Class_Id")]
        public List<Student> Stus { get; set; }
    }
    public class Student
    {
        [Key]
        public int Id { get; set; }
        public string UserName { get; set; }
        public int Age { get; set; }
        public int Class_Id { get; set; }
    }

    public class Person
    {
        public int PersonId { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; }
        public PersonPhoto Photo { get; set; }
    }
    /// <summary>
    /// 配置一对一的关系，两个实体类中都要相互引用，
    /// </summary>
    public class PersonPhoto
    {
        /// <summary>
        /// PersonId既是主键也是外键
        /// </summary>
        [Key, ForeignKey("PhotoOf")]
        public int PersonId { get; set; }
        public byte[] Photo { get; set; }
        public Person PhotoOf { get; set; }
    }
    public class User
    {
        [Key]
        public int UserId { get; set; }
        public string UserName { get; set; }
        public int Phone { get; set; }
    }
}
