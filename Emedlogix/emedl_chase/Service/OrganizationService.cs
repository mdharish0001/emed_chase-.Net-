using emedl_chase.DbModel;
using emedl_chase.Repository;

namespace emedl_chase.Service
{
    public class OrganizationService
    {
        IRepository<organization> _repository;

        public OrganizationService(IRepository<organization> repository)
        {
            _repository = repository;
        }
        public async Task<organization> Get(int id)
        {
            var data = await _repository.GetByIdAsync(id);
            return data;
        }
        public IQueryable<organization> GetAll(string name = null, string orgType = null, string email = null)
        {
            try
            {

                var data = _repository.TableNoTracking.AsQueryable();
                data = data.Where(a => !a.IsDeleted);
                if (!string.IsNullOrEmpty(name) && !string.IsNullOrWhiteSpace(name))
                {
                    data = data.Where(a => a.OrgName == name);
                }
                if (!string.IsNullOrEmpty(email) && !string.IsNullOrWhiteSpace(email))
                {
                    data = data.Where(a => a.Email == email);
                }
                if (!string.IsNullOrEmpty(orgType) && !string.IsNullOrWhiteSpace(orgType))
                {
                    name = name.ToLower();
                    data = data.Where(a => a.OrgType == orgType);
                }
                return data;
            }

            catch (Exception ex)
            {

                throw ex;
            }
        }
        public async Task<IEnumerable<organization>> Create(IEnumerable<organization> oEntityList)
        {
            if (oEntityList == null)
                throw new ArgumentNullException("userModel");
            oEntityList = await _repository.InsertAsync(oEntityList);
            return oEntityList;
        }

        public async Task<organization> Create(organization oEntity)
        {
            if (oEntity == null)
                throw new ArgumentNullException("userModel");

            oEntity = await _repository.InsertAsync(oEntity);
            return oEntity;
        }

        public void Update(organization oEntity)
        {
            _repository.Update(oEntity);
        }

        public IQueryable<organization> GetAllLineItems(long? documentId = null, string status = null, int? userid = null, int? clientUploadId = null, int? orgid = null, string unique_id = null)
        {
            var data = _repository.Table.AsQueryable();

            data = data.Where(a => !a.IsDeleted);

            if (documentId != null)
            {
                data = data.Where(a => a.Id == documentId);
            }

            return data;
        }
    }
}
