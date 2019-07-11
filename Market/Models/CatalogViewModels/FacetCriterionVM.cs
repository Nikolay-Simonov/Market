using System;
using System.Collections.Generic;

namespace Market.Models.CatalogViewModels
{
    public class FacetCriterionVM : IEquatable<FacetCriterionVM>
    {
        public FacetCriterionVM(string name, string bindingName = null)
        {
            Name = name;
            BindingName = bindingName;
        }

        public HashSet<string> All { get; set; } = new HashSet<string>();

        public HashSet<string> Selected { get; set; } = new HashSet<string>();

        public string Name { get; }

        public string BindingName { get; }

        #region HashCode Support

        public bool Equals(FacetCriterionVM other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            return ReferenceEquals(this, other)
                   || string.Equals(Name, other.Name, StringComparison.OrdinalIgnoreCase);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            return obj.GetType() == GetType() && Equals((FacetCriterionVM) obj);
        }

        public override int GetHashCode()
        {
            return Name != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(Name) : 0;
        }

        public static bool operator ==(FacetCriterionVM left, FacetCriterionVM right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(FacetCriterionVM left, FacetCriterionVM right)
        {
            return !Equals(left, right);
        }

        #endregion
    }
}