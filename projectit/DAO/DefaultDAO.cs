﻿using System;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using projectit.Models;
using System.Collections.Generic;

namespace projectit.DAO
{
    public abstract class DefaultDAO<T> where T : DefaultViewModel
    {
        protected DefaultDAO()
        {
            SetTable();
        }

        protected abstract SqlParameter[] CreateParams(T model);
        protected abstract SqlParameter[] UpdateParams(T model);
        protected abstract T MountModel(DataRow register);
        protected string Table { get; set; }
        protected abstract void SetTable();


        public virtual void Insert(T model)
        {
            HelperDAO.RunProc("spInsert_" + Table, CreateParams(model));
        }

        public virtual void Update(T model)
        {
            HelperDAO.RunProc("spUpdate_" + Table, UpdateParams(model));
        }

        public virtual void Delete(int id)
        {
            var p = new SqlParameter[]
            {
                new SqlParameter("id", id),
                new SqlParameter("id_name", Table.ToLower().Replace( "tb", "") + "_id"),
                new SqlParameter("table", Table)
            };
            HelperDAO.RunProc("spDelete", p);
        }

        public virtual T Query(int id)
        {
            var p = new SqlParameter[]
            {
                new SqlParameter("id", id),
                new SqlParameter("id_name", Table.ToLower().Replace( "tb", "") + "_id"),
                new SqlParameter("table", Table)
            };

            var table = HelperDAO.RunProcSelect("spQuery", p);
            if (table.Rows.Count == 0)
                return null;
            else
                return MountModel(table.Rows[0]);
        }

        public virtual List<T> List()
        {
            var p = new SqlParameter[]
            {
                new SqlParameter("table", Table),
                new SqlParameter("order", "1")
            };

            var table = HelperDAO.RunProcSelect("spList", p);
            List<T> list = new List<T>();
            foreach(DataRow register in table.Rows)
            {
                list.Add(MountModel(register));
            }

            return list;
        }

    }
}
