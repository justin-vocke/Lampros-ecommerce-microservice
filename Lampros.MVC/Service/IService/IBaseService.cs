using Lampros.MVC.Models;

namespace Lampros.MVC.Service.IService
{
    public interface IBaseService
    {
        Task<ResponseDto> SendAsync(RequestDto requestDto);
    }
}
