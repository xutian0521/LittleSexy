using LittleSexy.Common;
using LittleSexy.Model.DBModel;
using System;
using System.Collections.Generic;
using System.Text;
using Dapper;
using System.Threading.Tasks;
using LittleSexy.Model.ViewModel;

namespace LittleSexy.DAL
{
    [Inject]
    public class PageDAL
    {
        public async Task<t_Page> GetDataPageContentByPageIdAsync(int pageId)
        {
            
            var dataPage = await DB.Conn().QueryFirstOrDefaultAsync<t_Page>("SELECT * FROM LittleSexy.t_Page where id=@pageId;",new { pageId = pageId });
            return dataPage;
        }

        public async Task<IEnumerable< v_PageImages>> GetDataPageImagesByPageIdAsync(int pageId)
        {

            var dataPage = await DB.Conn().QueryAsync<v_PageImages>
                (@"SELECT  t_Page.Id,ImageRelativePath as ImagePath,t_Image.WidthPx as ImageWidth,t_Image.HeightPx as ImageHeight,t_DictionaryData.Id as ImageTypeId, DictionaryValue as ImageTypeName FROM t_Page 
                    join t_PageImageMapping  on t_Page.Id=t_PageImageMapping.PageId
                    join t_Image on t_PageImageMapping.ImageId=t_Image.Id
                    join t_DictionaryData on t_Image.ImageType=t_DictionaryData.Id 
                    join t_Dictionary on t_DictionaryData.DictionaryId=t_Dictionary.Id where t_Page.Id=@pageId;", new { pageId = pageId });
            return dataPage;
        }
    }

}
