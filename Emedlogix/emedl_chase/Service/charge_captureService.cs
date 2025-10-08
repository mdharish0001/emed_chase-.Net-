using emedl_chase.DbModel;
using emedl_chase.Repository;

namespace emedl_chase.Service
{
    public class charge_captureService
    {

        IRepository<charge_capture> _repository;

        public charge_captureService(IRepository<charge_capture> repository)
        {
            _repository = repository;
        }

        public async Task<charge_capture> Get(long id)
        {
            var data = await _repository.GetByIdAsync(id);
            return data;
        }


        public async Task<IEnumerable<charge_capture>> Create(IEnumerable<charge_capture> oEntityList)
        {
            if (oEntityList == null)
                throw new ArgumentNullException("client_uploadsModel");
            oEntityList = await _repository.InsertAsync(oEntityList);
            return oEntityList;
        }

        public void Update(charge_capture oEntity)
        {
            _repository.Update(oEntity);
        }

        public async Task<charge_capture> Create(charge_capture oEntity)
        {
            if (oEntity == null)
                throw new ArgumentNullException("client_uploadsModel");

            oEntity = await _repository.InsertAsync(oEntity);
            return oEntity;
        }

        public IQueryable<charge_capture> GetAll(string patientname = null,string practice = null, string cpt = null, int? claim_id=null ,string? patient_id=null,int? encounter_id = null,int?org_id = null,DateTime?dos=null)

        {

            var data= _repository.TableNoTracking.AsQueryable();

            data = data.Where(a => a.isdelete != true);

            if (patientname != null)
            {
                data=data.Where(a=>a.patient_name == patientname);
            } 
            if (practice != null)
            {
                data=data.Where(a=>a.practice == practice);
            }

            if (cpt != null)
            {
                data=data.Where(a=>a.cpt == cpt);
            }
            if (claim_id >0)
            {
                data = data.Where(a => a.claim_id == claim_id);
            }

            if (patient_id != null)
            {
                data = data.Where(a => a.patient_id == patient_id);
            }
            if (encounter_id > 0)
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
