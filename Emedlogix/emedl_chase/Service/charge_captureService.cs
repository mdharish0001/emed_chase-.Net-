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
    }
}
