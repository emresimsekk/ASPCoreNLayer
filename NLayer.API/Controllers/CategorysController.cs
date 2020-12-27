using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NLayer.API.Dtos;
using NLayer.Core.Entity;
using NLayer.Core.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NLayer.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategorysController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly IMapper _mapper;
        public CategorysController(ICategoryService categoryService,IMapper mapper)
        {
            _categoryService = categoryService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _categoryService.GetAllAsync();
            return Ok(_mapper.Map<IEnumerable<CategoryDto>>(categories));
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int Id)
        {
            var category = await _categoryService.GetByIdAsync(Id);
            return Ok(_mapper.Map<CategoryDto>(category));
        }


        [HttpPost]
        public async Task<IActionResult> Save([FromForm] CategoryDto categoryDto)
        {
            var newCategory=await _categoryService.AddAsync(_mapper.Map<Category>(categoryDto));
            return Created(string.Empty,_mapper.Map<CategoryDto>(newCategory));
        }


        [HttpPut]
        public  IActionResult Update([FromForm] CategoryDto categoryDto)
        {
            var category = _categoryService.Update(_mapper.Map<Category>(categoryDto));

            return NoContent();
        }


        [HttpDelete("{id}")]
        public IActionResult Remove(int id)
        {
            var category = _categoryService.GetByIdAsync(id).Result;
            _categoryService.Remove(category);
            return NoContent();
        }


        [HttpGet("{id}/products")]
        public async Task<IActionResult> GetWithPorductsById(int Id)
        {
            var category = await _categoryService.GetWithProductsByIdAsync(Id);

            return Ok(_mapper.Map<CategoryWithProductDto>(category));

        }
    }
}
