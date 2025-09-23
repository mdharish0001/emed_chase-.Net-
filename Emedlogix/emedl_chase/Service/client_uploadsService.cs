

using Newtonsoft.Json;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using emedl_chase.DbModel;
using emedl_chase.Repository;
using static System.Collections.Specialized.BitVector32;

namespace emedl_chase.Service;

public class client_uploadsService
{
    IRepository<client_uploads> _repository;

    public client_uploadsService(IRepository<client_uploads> repository)
    {
        _repository = repository;
    }
    public IQueryable<client_uploads> GetAll(string batch = null, string status = null, int? org_id = null, int? specialties = null, int? clientId = null, DateTime? created_date = null, DateTime? receivedDateFrom = null, DateTime? receivedDateTo = null)
    {
        var data = _repository.TableNoTracking.AsQueryable();

        data = data.Where(d => !d.isdelete);

        if (specialties != null)
        {
            data = data.Where(a => a.specialties == specialties);
        }
        if (org_id > 0)
        {
            data = data.Where(a => a.org_id == org_id);
        }
        //if (!string.IsNullOrWhiteSpace(batch))
        //{
        //    data = data.Where(a => a.batch.Trim().ToLower() == batch.Trim().ToLower());
        //}
        if (!string.IsNullOrWhiteSpace(batch))
        {
            data = data.Where(a => a.batch.Contains(batch));
        }
        if (!string.IsNullOrWhiteSpace(status))
        {
            data = data.Where(a => a.status == status);
        }
        if (clientId != null)
        {
            data = data.Where(a => a.id == clientId);
        }

        if (created_date != null)
        {
            data = data.Where(a => a.created_on.Value.Date == created_date.Value.Date);
        }

        if (receivedDateFrom != null && receivedDateTo != null)
        {
            data = data.Where(d => d.created_on.Value.Date >= receivedDateFrom.Value.Date && d.created_on.Value.Date <= receivedDateTo.Value.Date);
        }
        return data;
    }

    public IQueryable<client_uploads> GetAllLineItems(string batch = null, string status = null, int? org_id = null, int? specialties = null,int? clientId = null, DateTime? created_date = null, DateTime? receivedDateFrom = null, DateTime? receivedDateTo = null)
    {
        var data = _repository.Table.AsQueryable();

        if (specialties != null)
        {
            data = data.Where(a => a.specialties == specialties);
        }
        if (org_id > 0)
        {
            data = data.Where(a => a.org_id == org_id);
        }
        //if (!string.IsNullOrWhiteSpace(batch))
        //{
        //    data = data.Where(a => a.batch.Trim().ToLower() == batch.Trim().ToLower());
        //}
        if (!string.IsNullOrWhiteSpace(batch))
        {
            data = data.Where(a => a.batch.Contains(batch));
        }
        if (!string.IsNullOrWhiteSpace(status))
        {
            data = data.Where(a => a.status == status);
        }
        if (clientId != null)
        {
            data = data.Where(a => a.id == clientId);
        }

        if(created_date != null)
        {
            data = data.Where(a => a.created_on.Value.Date == created_date.Value.Date);
        }

        if(receivedDateFrom != null && receivedDateTo != null)
        {
            data = data.Where(d => d.created_on.Value.Date >= receivedDateFrom.Value.Date && d.created_on.Value.Date <= receivedDateTo.Value.Date);
        }



        return data;
    }


    public async Task<client_uploads> Get(long id)
    {
        var data = await _repository.GetByIdAsync(id);
        return data;
    }
    public async Task<IEnumerable<client_uploads>> Create(IEnumerable<client_uploads> oEntityList)
    {
        if (oEntityList == null)
            throw new ArgumentNullException("client_uploadsModel");
        oEntityList = await _repository.InsertAsync(oEntityList);
        return oEntityList;
    }

    public void Update(client_uploads oEntity)
    {
        _repository.Update(oEntity);
    }

    public async Task<client_uploads> Create(client_uploads oEntity)
    {
        if (oEntity == null)
            throw new ArgumentNullException("client_uploadsModel");

        oEntity = await _repository.InsertAsync(oEntity);
        return oEntity;
    }
}
