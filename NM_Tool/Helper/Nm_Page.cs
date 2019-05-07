using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NM_Tool.Helper
{
    /// <summary>
    /// 分页类
    /// </summary>
    public class Nm_Page<T>
    {
        //当前页数
        private int _pageIndex;
        public int PageIndex
        {
            get
            {
                if (_pageIndex < 1)
                    return 1;
                return _pageIndex;
            }
            set { _pageIndex = value; }
        }

        //总数量
        private int _pageCount;
        public int PageCount
        {
            get {
                if (_list!=null&&_pageCount != _list.Count)
                    _pageCount = _list.Count;
                return _pageCount; 
            }
            set
            {
                _pageMax = _pageCount % _pageSize == 0 ? _pageCount / _pageSize :
                   (_pageCount / _pageSize) + 1;

                _pageCount = value;
            }
        }

        //最大页数
        private int _pageMax;
        public int PageMax
        {
            get
            {
                _pageMax = _pageCount % _pageSize == 0 ? _pageCount / _pageSize :
                 (_pageCount / _pageSize) + 1;

                return _pageMax;
            }
            set { _pageMax = value; }
        }

        //分页数量
        private int _pageSize;
        public int PageSize
        {
            get { return _pageSize; }
            set { _pageSize = value; }
        }

        //集合
        private List<T> _list;
        public List<T> List
        {
            get
            {
                if (_list == null)
                    _list = new List<T>();
                return _list;
            }
            set
            {
                _pageCount = value.Count;
                _list = value;
            }
        }

        /// <summary>
        /// 首页
        /// </summary>
        public void HomPage()
        {
            _pageIndex = 1;
        }

        /// <summary>
        /// 上一页
        /// </summary>
        public void PrePage()
        {
            if (_pageIndex > 1)
                _pageIndex--;
            else
                HomPage();
        }

        /// <summary>
        /// 下一页
        /// </summary>
        public void NextPage()
        {
            if (_pageIndex < _pageMax)
                _pageIndex++;
            else
                TraPage();
        }

        /// <summary>
        /// 尾页
        /// </summary>
        public void TraPage()
        {
            _pageIndex = _pageMax;
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <returns>返回当前页数数据</returns>
        public List<T> GetList()
        {
            return _list.Skip((_pageIndex - 1) * _pageSize).Take(_pageSize).ToList();
        }

        /// <summary>
        /// 获取某个对象
        /// </summary>
        /// <param name="index">对象下表</param>
        /// <returns>对象</returns>
        public T GetObj(int index)
        {
            try
            {
                return _list[index];
            }
            catch (Exception ex)
            {
                Log.writeLog(ex);
                return default(T);
            }

        }

        /// <summary>
        /// 获取当前页的某个对象
        /// </summary>
        /// <param name="index">当前页的对象下表</param>
        /// <returns>对象</returns>
        public T GetObjPage(int index)
        {
            try
            {
                return _list.Skip((_pageIndex - 1) * _pageSize).Take(_pageSize).ToList()[index];
            }
            catch (Exception ex)
            {
                Log.writeLog(ex);
                return default(T);
            }

        }





    }
}
