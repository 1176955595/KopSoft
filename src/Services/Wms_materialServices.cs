using IRepository;
using IServices;
using SqlSugar;
using System;
using YL.Core.Entity;
using YL.Utils.Excel;
using YL.Utils.Extensions;
using YL.Utils.Json;
using YL.Utils.Pub;
using YL.Utils.Table;

namespace Services
{
    public class Wms_materialServices : BaseServices<Wms_material>, IWms_materialServices
    {
        private readonly IWms_materialRepository _repository;
        private readonly SqlSugarClient _client;

        public Wms_materialServices(SqlSugarClient client, IWms_materialRepository repository) : base(repository)
        {
            _client = client;
            _repository = repository;
        }

        public string PageList(Bootstrap.BootstrapParams bootstrap)
        {
            int totalNumber = 0;
            if (bootstrap.offset != 0)
            {
                bootstrap.offset = bootstrap.offset / bootstrap.limit + 1;
            }
            var query = _client.Queryable<Wms_material, Sys_dict, Sys_dict, Wms_storagerack, Wms_reservoirarea, Wms_warehouse, Sys_user, Sys_user>
                ((s, t, ut, r, k, w, c, u) => new object[] {
                   JoinType.Left,s.MaterialType==t.DictId,
                   JoinType.Left,s.Unit==ut.DictId,
                   JoinType.Left,s.StoragerackId==r.StorageRackId,
                   JoinType.Left,s.ReservoirAreaId==k.ReservoirAreaId,
                   JoinType.Left,s.WarehouseId==w.WarehouseId,
                   JoinType.Left,s.CreateBy==c.UserId,
                   JoinType.Left,s.ModifiedBy==u.UserId,
                 })
                 .Where((s, t, ut, r, k, w, c, u) => s.IsDel == 1 && t.IsDel == 1 && ut.IsDel == 1 && r.IsDel == 1 && k.IsDel == 1 && w.IsDel == 1)
                 .Select((s, t, ut, r, k, w, c, u) => new
                 {
                     MaterialId = s.MaterialId.ToString(),
                     s.MaterialNo,
                     s.MaterialName,
                     r.StorageRackNo,
                     r.StorageRackName,
                     k.ReservoirAreaNo,
                     k.ReservoirAreaName,
                     w.WarehouseNo,
                     w.WarehouseName,
                     MaterialType = t.DictName,
                     Unit = ut.DictName,
                     s.Qty,
                     s.ExpiryDate,
                     s.IsDel,
                     s.Remark,
                     CName = c.UserNickname,
                     s.CreateDate,
                     UName = u.UserNickname,
                     s.ModifiedDate
                 }).MergeTable();
            if (!bootstrap.search.IsEmpty())
            {
                query.Where((s) => s.MaterialNo.Contains(bootstrap.search) || s.MaterialName.Contains(bootstrap.search));
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

        public byte[] ExportList(Bootstrap.BootstrapParams bootstrap)
        {
            bootstrap.sort = "����ʱ��";
            bootstrap.order = "desc";
            var query = _client.Queryable<Wms_material, Sys_dict, Sys_dict, Wms_storagerack, Wms_reservoirarea, Wms_warehouse, Sys_user, Sys_user>
                ((s, t, ut, r, k, w, c, u) => new object[] {
                   JoinType.Left,s.MaterialType==t.DictId,
                   JoinType.Left,s.Unit==ut.DictId,
                   JoinType.Left,s.StoragerackId==r.StorageRackId,
                   JoinType.Left,s.ReservoirAreaId==k.ReservoirAreaId,
                   JoinType.Left,s.WarehouseId==w.WarehouseId,
                   JoinType.Left,s.CreateBy==c.UserId,
                   JoinType.Left,s.ModifiedBy==u.UserId,
                 })
                 .Where((s, t, ut, r, k, w, c, u) => s.IsDel == 1 && t.IsDel == 1 && ut.IsDel == 1 && r.IsDel == 1 && k.IsDel == 1 && w.IsDel == 1)
                 .Select((s, t, ut, r, k, w, c, u) => new
                 {
                     ���ϱ�� = s.MaterialNo,
                     �������� = s.MaterialName,
                     ��λ��� = ut.DictName,
                     ���Ϸ��� = t.DictName,
                     ��ȫ��� = s.Qty,
                     ��Ч�� = s.ExpiryDate,
                     ���ܱ�� = r.StorageRackNo,
                     �������� = r.StorageRackName,
                     ������� = k.ReservoirAreaNo,
                     �������� = k.ReservoirAreaName,
                     �ֿ��� = w.WarehouseNo,
                     �ֿ����� = w.WarehouseName,
                     ��ע = s.Remark,
                     ������ = c.UserNickname,
                     ����ʱ�� = s.CreateDate,
                     �޸��� = u.UserNickname,
                     �޸�ʱ�� = s.ModifiedDate
                 }).MergeTable();
            if (!bootstrap.datemin.IsEmpty() && !bootstrap.datemax.IsEmpty())
            {
                query.Where(s => s.����ʱ�� > bootstrap.datemin.ToDateTimeB() && s.����ʱ�� <= bootstrap.datemax.ToDateTimeE());
            }
            if (bootstrap.order.Equals("desc", StringComparison.OrdinalIgnoreCase))
            {
                query.OrderBy($"MergeTable.{bootstrap.sort} desc");
            }
            else
            {
                query.OrderBy($"MergeTable.{bootstrap.sort} asc");
            }
            var list = query.ToList();
            var buffer = NpoiUtil.Export(list, ExcelVersion.V2007);
            return buffer;
        }
    }
}