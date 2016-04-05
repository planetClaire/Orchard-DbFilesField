using System;
using System.ComponentModel.DataAnnotations;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;

namespace DbFilesField.Attributes {

    /// <summary>
    /// http://orchardpros.net/tickets/6994
    /// </summary>
    public class BinaryLengthMaxAttribute : StringLengthAttribute {
        public BinaryLengthMaxAttribute() : base(Int32.MaxValue) {}
    }

    public class BinaryLengthConvention : AttributePropertyConvention<StringLengthAttribute> {
        protected override void Apply(StringLengthAttribute attribute, IPropertyInstance instance) {
            instance.CustomSqlType("varbinary(MAX)");
            instance.Length(attribute.MaximumLength);
        }
    }
}