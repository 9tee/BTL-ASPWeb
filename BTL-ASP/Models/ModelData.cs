namespace BTL_ASP.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Text.RegularExpressions;
    using PagedList;

    public partial class ModelData : DbContext
    {
        public ModelData()
            : base("name=ModelData")
        {
        }

        public virtual DbSet<AnhSanPham> AnhSanPhams { get; set; }
        public virtual DbSet<GioHang> GioHangs { get; set; }
        public virtual DbSet<KhachHang> KhachHangs { get; set; }
        public virtual DbSet<Loai> Loais { get; set; }
        public virtual DbSet<SanPham> SanPhams { get; set; }
        public virtual DbSet<SanPhamGioHang> SanPhamGioHangs { get; set; }
        public virtual DbSet<sysdiagram> sysdiagrams { get; set; }
        public virtual DbSet<LichSuMuaHang> LichSuMuaHangs { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AnhSanPham>()
                .Property(e => e.LinkAnh)
                .IsUnicode(false);

            modelBuilder.Entity<GioHang>()
                .Property(e => e.Email)
                .IsUnicode(false);

            modelBuilder.Entity<GioHang>()
                .Property(e => e.SDT)
                .IsUnicode(false);

            modelBuilder.Entity<GioHang>()
                .Property(e => e.TongTien)
                .HasPrecision(18, 0);

            modelBuilder.Entity<GioHang>()
                .HasMany(e => e.SanPhamGioHangs)
                .WithRequired(e => e.GioHang)
                .HasForeignKey(e => e.IDGH)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<KhachHang>()
                .Property(e => e.Email)
                .IsUnicode(false);

            modelBuilder.Entity<KhachHang>()
                .Property(e => e.Password)
                .IsUnicode(false);

            modelBuilder.Entity<KhachHang>()
                .Property(e => e.SDT)
                .IsUnicode(false);

            modelBuilder.Entity<KhachHang>()
                .HasMany(e => e.GioHangs)
                .WithOptional(e => e.KhachHang)
                .HasForeignKey(e => e.IDKH);

            modelBuilder.Entity<Loai>()
                .HasMany(e => e.SanPhams)
                .WithOptional(e => e.Loai)
                .HasForeignKey(e => e.MaLoai);

            modelBuilder.Entity<SanPham>()
                .Property(e => e.Gia)
                .HasPrecision(18, 0);

            modelBuilder.Entity<SanPham>()
                .HasMany(e => e.AnhSanPhams)
                .WithOptional(e => e.SanPham)
                .HasForeignKey(e => e.IDSP);

            modelBuilder.Entity<SanPham>()
                .HasMany(e => e.SanPhamGioHangs)
                .WithRequired(e => e.SanPham)
                .HasForeignKey(e => e.IDSP)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<SanPhamGioHang>()
                .Property(e => e.DonGia)
                .HasPrecision(18, 0);

            modelBuilder.Entity<SanPhamGioHang>()
                .Property(e => e.ThanhTien)
                .HasPrecision(18, 0);
            modelBuilder.Entity<LichSuMuaHang>()
                .Property(e => e.ThanhTien)
                .HasPrecision(18, 0);
        }

    }

    public class FSanPham
    {
        private ModelData context = new ModelData();
        public IQueryable<SanPham> SanPhams
        {
            get { return context.SanPhams; }
        }

        public SanPham FindSanPham(int id)
        {
            return context.SanPhams.Find(id);
        }
        public IEnumerable<SanPham> GetDanhSachSP(int MaLoai, int page, int pageSize)
        {
            return context.SanPhams.OrderByDescending(x => x.ID).Where(x => x.MaLoai == MaLoai).ToPagedList(page, pageSize);
        }
        public IEnumerable<SanPham> GetSanPhams(int page, int pageSize)
        {
            return context.SanPhams.OrderByDescending(x => x.ID).ToPagedList(page, pageSize);
        }
    }

    public class FLoaiSanPham
    {
        private ModelData context = new ModelData();
        public IQueryable<Loai> SanPhams
        {
            get { return context.Loais; }
        }
        public Loai FindLoai(string TenLoai)
        {
            Loai myLoai = context.Loais.SingleOrDefault(loai => loai.TenLoai == TenLoai);
            return myLoai;
        }
    }

    public class FKhachHang
    {
        private ModelData context = new ModelData();
        public KhachHang TimKhachHang(string id, string pass)
        {
            return context.KhachHangs.Where(a => a.Email.Equals(id) && a.Password.Equals(pass)).FirstOrDefault();
        }
        public bool KiemTraEmail(string email)
        {
            return Regex.IsMatch(email, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);
        }
        public void LayLaiMatKhau(string email)
        {
            SqlParameter[] idParam = {
                new SqlParameter {ParameterName = "Email",Value = email}};
            context.Database.ExecuteSqlCommand("LayLaiMatKhau @Email", idParam);
        }

        public void ThemKhachHang(KhachHang khach)
        {
            khach.ID = context.KhachHangs.Count() + 1;
            khach.DangKyNgay = DateTime.Now;
            context.KhachHangs.Add(khach);
            context.SaveChanges();
        }

        public bool ThayDoiMatKhau(int id,string password)
        {
            KhachHang khach = context.KhachHangs.Where(x => x.ID == id).FirstOrDefault();
            if(khach != null)
            {
                khach.Password = password;
                context.SaveChanges();
                return true;
            }   
            else
            {
                return false;
            }    
        }
    }

    public class FAnhSanPham
    {
        private ModelData context = new ModelData();
        public List<AnhSanPham> GetSilde()
        {
            List<AnhSanPham> lasp = new List<AnhSanPham>();
            lasp = context.AnhSanPhams.SqlQuery("GetSilde").ToList<AnhSanPham>();
            return lasp;
        }
        public List<AnhSanPham> GetRanKeyBoard()
        {
            List<AnhSanPham> lasp = new List<AnhSanPham>();
            lasp = context.AnhSanPhams.SqlQuery("GetRanKeyBoard").ToList<AnhSanPham>();
            return lasp;
        }
        public List<AnhSanPham> GetRanOther()
        {
            List<AnhSanPham> lasp = new List<AnhSanPham>();
            lasp = context.AnhSanPhams.SqlQuery("GetRanOther").ToList<AnhSanPham>();
            return lasp;
        }
    }

    public class FLichSuMuaHang
    {
        private ModelData context = new ModelData();
        public List<LichSuMuaHang> LichSuKhachHang(int id)
        {
            List<LichSuMuaHang> lichSuMuaHangs = new List<LichSuMuaHang>();
            lichSuMuaHangs = context.LichSuMuaHangs.Where(a => a.IDKH == id).ToList();
            return lichSuMuaHangs;
        }
    }
}
