using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using emedl_chase.DbModel;
using emedl_chase.Repository;

namespace emedl_chase.Service
{
    public class payment_postService
    {

        IRepository<payment_posting> _repository;

        public payment_postService(IRepository<payment_posting> repository)
        {
            _repository = repository;
        }

        public async Task<payment_posting> Get(long id)
        {
            var data = await _repository.GetByIdAsync(id);
            return data;
        }


        public async Task<IEnumerable<payment_posting>> Create(IEnumerable<payment_posting> oEntityList)
        {
            if (oEntityList == null)
                throw new ArgumentNullException("client_uploadsModel");
            oEntityList = await _repository.InsertAsync(oEntityList);
            return oEntityList;
        }

        public void Update(payment_posting oEntity)
        {
            _repository.Update(oEntity);
        }

        public async Task SaveChanges()
        {
            await _repository.SaveAsync();
        }

        public async Task<payment_posting> Create(payment_posting oEntity)
        {
            if (oEntity == null)
                throw new ArgumentNullException("client_uploadsModel");

            oEntity = await _repository.InsertAsync(oEntity);
            return oEntity;
        }

        public IQueryable<payment_posting> GetAll(string patientname = null, string practice = null, string cpt = null, string claim_id = null, string? patient_id = null, string encounter_id = null, int? org_id = null, DateTime? dos = null)

        {

            var data = _repository.TableNoTracking.AsQueryable();

            data = data.Where(a => a.isdelete != true);

            if (patientname != null)
            {
                data = data.Where(a => a.patient_name == patientname);
            }
            if (practice != null)
            {
                data = data.Where(a => a.practice == practice);
            }

            if (cpt != null)
            {
                data = data.Where(a => a.cpt == cpt);
            }
            if (claim_id != null)
            {
                data = data.Where(a => a.claim_id == claim_id);
            }

            if (patient_id != null)
            {
                data = data.Where(a => a.patient_id == patient_id);
            }
            if (encounter_id != null)
            {
                data = data.Where(a => a.encounter_id == encounter_id);
            }
            if (org_id > 0)
            {
                data = data.Where(a => a.org_id == org_id);
            }
            if (dos != null)
            {
                data = data.Where(d => d.dos.Value.Date >= dos.Value.Date);
            }

            return data;

        }

        public IQueryable<payment_posting> GetAllTable(string patientname = null, string practice = null, string cpt = null, string claim_id = null, string? patient_id = null, string encounter_id = null, int? org_id = null, DateTime? dos = null)

        {

            var data = _repository.Table.AsQueryable();

            data = data.Where(a => a.isdelete != true);

            if (patientname != null)
            {
                data = data.Where(a => a.patient_name == patientname);
            }
            if (practice != null)
            {
                data = data.Where(a => a.practice == practice);
            }

            if (cpt != null)
            {
                data = data.Where(a => a.cpt == cpt);
            }
            if (claim_id != null)
            {
                data = data.Where(a => a.claim_id == claim_id);
            }

            if (patient_id != null)
            {
                data = data.Where(a => a.patient_id == patient_id);
            }
            if (encounter_id != null)
            {
                data = data.Where(a => a.encounter_id == encounter_id);
            }
            if (org_id > 0)
            {
                data = data.Where(a => a.org_id == org_id);
            }
            if (dos != null)
            {
                data = data.Where(d => d.dos.Value.Date >= dos.Value.Date);
            }

            return data;

        }
    }
}
