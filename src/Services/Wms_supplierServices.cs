using IRepository;
using IServices;
using SqlSugar;
using System;
using YL.Core.Entity;
using YL.Utils.Extensions;
using YL.Utils.Json;
using YL.Utils.Table;

namespace Services
{
    public class Wms_supplierServices : BaseServices<Wms_supplier>, IWms_supplierServices
    {
        private readonly IWms_supplierRepository _repository;
        private readonly SqlSugarClient _client;

        public Wms_supplierServices(SqlSugarClient client, IWms_supplierRepository repository) : base(repository)
        {
            _repository = repository;
            _client = client;
        }

        public string PageList(Bootstrap.BootstrapParams bootstrap)
        {
            int totalNumber = 0;
            if (bootstrap.offset != 0)
            {
                bootstrap.offset = bootstrap.offset / bootstrap.limit + 1;
            }
            var query = _client.Queryable<Wms_supplier, Sys_user, Sys_user>
                ((s, c, u) => new object[] {
                   JoinType.Left,s.CreateBy==c.UserId,
                   JoinType.Left,s.ModifiedBy==u.UserId
                 })
                 .Select((s, c, u) => new
                 {
                     SupplierId = s.SupplierId.ToString(),
                     s.SupplierNo,
                     s.SupplierName,
                     s.Address,
                     s.SupplierPerson,
                     s.SupplierLevel,
                     s.Tel,
                     s.Email,
                     s.IsDel,
                     s.Remark,
                     CName = c.UserNickname,
                     s.CreateDate,
                     UName = u.UserNickname,
                     s.ModifiedDate
                 }).MergeTable().Where((s) => s.IsDel == 1);
            if (!bootstrap.search.IsEmpty())
            {
                query.Where((s) => s.SupplierName.Contains(bootstrap.search) || s.SupplierNo.Contains(bootstrap.search));
            }
            if (!bootstrap.datemin.IsEmpty() && !bootstrap.datemax.IsEmpty())
            {
                query.Where(s => s.CreateDate > bootstrap.datemin.ToDateTimeB() && s.CreateDate <= bootstrap.datemax.ToDateTimeE());
            }
            if (bootstrap.order.Equals("desc", StringComparison.OrdinalIgnoreCase))
            {
                query.OrderBy($"MergeTable.{bootstrap.sort} desc");
            }
            else
            {
                query.OrderBy($"MergeTable.{bootstrap.sort} asc");
            }
            var list = query.ToPageList(bootstrap.offset, bootstrap.limit, ref totalNumber);
            return Bootstrap.GridData(list, totalNumber).JilToJson();
        }
    }
}