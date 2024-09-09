using Application.IRepository;
using Application.IService;
using Application.ServiceResponse;
using Application.Utils;
using Application.ViewModels;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service
{
    public class ImageService :IImageService
    {
        private readonly IImageRepo _imageRepo;
        private readonly IMapper _mapper;


        public ImageService(IImageRepo imageRepo, IMapper mapper)
        {
            _imageRepo = imageRepo ?? throw new ArgumentNullException(nameof(imageRepo));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }


        public async Task<ServiceResponse<PaginationModel<ProductImageDTO>>> GetAllImageInfors(int page, int pageSize,
            string search, string sort)
        {
            var response = new ServiceResponse<PaginationModel<ProductImageDTO>>();

            try
            {
                var images = await _imageRepo.GetAllImageInfors();
                if (!string.IsNullOrEmpty(search) && int.TryParse(search, out var searchProductId))
                {
                    images = images.Where(p => p.ProductId == searchProductId);
                }

                images = sort.ToLower() switch
                {
                    "publicid" => images.OrderBy(p => p.PublicId),
                    "productid" => images.OrderBy(p => p.ProductId),
                    _ => images.OrderBy(p => p.Id)
                };
                var imageDTOs = _mapper.Map<IEnumerable<ProductImageDTO>>(images); // Map images to ProductImageDTO

                // Apply pagination
                var paginationModel = await Pagination.GetPaginationIENUM(imageDTOs, page, pageSize);

                response.Data = paginationModel;
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to retrieve image infors: {ex.Message}";
            }

            return response;
        }
        public async Task<ServiceResponse<ProductImageDTO>> GetImageInforById(int id)
        {
            var serviceResponse = new ServiceResponse<ProductImageDTO>();

            try
            {
                var image = await _imageRepo.GetImageInforById(id);
                if (image == null)
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = "Zodiac product not found";
                }
                else
                {
                    var imageDTO = _mapper.Map<ProductImageDTO>(image);
                    serviceResponse.Data = imageDTO;
                    serviceResponse.Success = true;
                }
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }

            return serviceResponse;
        }
        //public async Task<ServiceResponse<string>> DeleteProductImage(int id)
        //{
        //    var serviceResponse = new ServiceResponse<string>();

        //    try
        //    {
        //        await _imageRepo.DeleteProductImage(id);
        //        serviceResponse.Success = true;
        //        serviceResponse.Message = "Product image deleted successfully";
        //    }
        //    catch (Exception ex)
        //    {
        //        serviceResponse.Success = false;
        //        serviceResponse.Message = "Failed to delete product image: " + ex.Message;
        //    }

        //    return serviceResponse;
        //}
    }
}

