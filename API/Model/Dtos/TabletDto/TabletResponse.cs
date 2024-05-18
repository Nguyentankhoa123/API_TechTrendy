using API.Model.Entity;
using API.Services.Common;

namespace API.Model.Dtos.TabletDto
{
    public class TabletResponse
    {
        public int Id { get; set; }

        public virtual Category Category { get; set; }

        public virtual Brand Brand { get; set; }

        public string Name { get; set; }
        public string OS { get; set; }
        public string CPU { get; set; }

        public string Screen { get; set; }

        public string RAM { get; set; }
        public string Storage { get; set; }

        public List<string> Ports { get; set; }
        public string Design { get; set; }

        public string Weight { get; set; }

        public double Price { get; set; }

        public List<string> PictureUrls { get; set; }

        public string Description { get; set; }

        public int ReleaseDate { get; set; }
        public int Quantity { get; set; }
        public string Connectivity { get; set; }
        public string Sim { get; set; }
        public string RearCamera { get; set; }

        public string BackCamera { get; set; }
        public string Battery { get; set; }
    }

    public class TabletObjectResponse : ObjectResponse<TabletResponse> { }
    public class TabletListObjectResponse : ObjectResponse<List<TabletResponse>> { }
}
