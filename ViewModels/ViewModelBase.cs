using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace WpfApplication1.ViewModels
{
    public class ViewModelBase : INotifyPropertyChanged, IDataErrorInfo
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void RaisePropertyChanged<T>(Expression<Func<T>> propertyExpression)
        {
            if (propertyExpression == null)
                throw new ArgumentNullException("propertyExpression");

            MemberExpression memberExpression = propertyExpression.Body as MemberExpression;
            if (memberExpression == null)
                throw new ArgumentException("ラムダ式の内容がプロパティになっていません", "propertyExpression");

            RaisePropertyChanged(memberExpression.Member.Name);
        }

        protected void SetError(string message, [CallerMemberName] string propertyName = "")
        {
            errors[propertyName] = message;
        }

        protected void ClearError([CallerMemberName] string propertyName = "")
        {
            errors.Remove(propertyName);
        }

        protected bool HasError()
        {
            return errors.Any();
        }

        #region IDataErrorInfo実装

        private Dictionary<string, string> errors = new Dictionary<string, string>();

        string IDataErrorInfo.Error
        {
            get
            {
                return null;
            }
        }

        string IDataErrorInfo.this[string columnName]
        {
            get 
            {
                if (errors.ContainsKey(columnName))
                   return errors[columnName];
                return null;
            }
        }
        #endregion

    }
}
