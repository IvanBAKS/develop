using CromosList.Models;
using Microsoft.EntityFrameworkCore;

namespace CromosList.Data;

public class CromosListDbContext : DbContext
{
    public CromosListDbContext(DbContextOptions<CromosListDbContext> options)
        : base(options)
    {
    }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<UsuarioCromo> UsuariosCromo => Set<UsuarioCromo>();
    public DbSet<Cromo> Cromos => Set<Cromo>();
    public DbSet<Jugador> Jugadores => Set<Jugador>();
    public DbSet<Equipo> Equipos => Set<Equipo>();
    public DbSet<Edicion> Ediciones => Set<Edicion>();
    public DbSet<Publicacion> Publicaciones => Set<Publicacion>();
    public DbSet<TipoCromo> TiposCromo => Set<TipoCromo>();
    public DbSet<Coleccion> Colecciones => Set<Coleccion>();
    public DbSet<Editorial> Editoriales => Set<Editorial>();
    public DbSet<Album> Albums => Set<Album>();
    public DbSet<AlbumEquipo> AlbumEquipos => Set<AlbumEquipo>();
    public DbSet<AlbumTipoCromo> AlbumTiposCromo => Set<AlbumTipoCromo>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // USUARIO
        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.ToTable("usuario");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Id)
                .HasColumnName("id");

            entity.Property(x => x.Nombre)
                .HasColumnName("nombre");

            entity.Property(x => x.Email)
                .HasColumnName("email");

            entity.Property(x => x.Contrasena)
                .HasColumnName("contrasena");

            entity.Property(x => x.EsAdmin)
                .HasColumnName("es_admin");
        });

        // EQUIPO
        modelBuilder.Entity<Equipo>(entity =>
        {
            entity.ToTable("equipo");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Id)
                .HasColumnName("id");

            entity.Property(x => x.Nombre)
                .HasColumnName("nombre");
        });

        // JUGADOR
        modelBuilder.Entity<Jugador>(entity =>
        {
            entity.ToTable("jugador");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Id)
                .HasColumnName("id");

            entity.Property(x => x.Nombre)
                .HasColumnName("nombre");

            entity.Property(x => x.NombreCompleto)
                .HasColumnName("nombre_completo");

            // Relación N:M con Equipo (un jugador puede pertenecer a n equipos).
            entity.HasMany(x => x.Equipos)
                .WithMany(x => x.Jugadores)
                .UsingEntity<Dictionary<string, object>>(
                    "equipo_jugador",
                    x => x.HasOne<Equipo>()
                        .WithMany()
                        .HasForeignKey("equipo_id"),
                    x => x.HasOne<Jugador>()
                        .WithMany()
                        .HasForeignKey("jugador_id"),
                    x =>
                    {
                        x.ToTable("equipo_jugador");
                        x.HasKey("equipo_id", "jugador_id");
                    });
        });

        // EDICION
        modelBuilder.Entity<Edicion>(entity =>
        {
            entity.ToTable("edicion");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Id)
                .HasColumnName("id");

            entity.Property(x => x.Nombre)
                .HasColumnName("nombre");
        });

        // PUBLICACION
        modelBuilder.Entity<Publicacion>(entity =>
        {
            entity.ToTable("publicacion");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Id)
                .HasColumnName("id");

            entity.Property(x => x.Temporada)
                .HasColumnName("temporada");

            entity.Property(x => x.Nombre)
                .HasColumnName("nombre");

            entity.Property(x => x.FechaPublicacion)
                .HasColumnName("fecha_publicacion");

            entity.Property(x => x.EdicionId)
                .HasColumnName("edicion_id");

            entity.HasOne(x => x.Edicion)
                .WithMany(x => x.Publicaciones)
                .HasForeignKey(x => x.EdicionId);
        });

        // TIPO_CROMO
        modelBuilder.Entity<TipoCromo>(entity =>
        {
            entity.ToTable("tipo_cromo");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Id)
                .HasColumnName("id");

            entity.Property(x => x.Nombre)
                .HasColumnName("nombre");
        });

        // COLECCION
        modelBuilder.Entity<Coleccion>(entity =>
        {
            entity.ToTable("coleccion");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Id)
                .HasColumnName("id");

            entity.Property(x => x.Nombre)
                .HasColumnName("nombre");
        });

        // CROMO
        modelBuilder.Entity<Cromo>(entity =>
        {
            entity.ToTable("cromo");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Id)
                .HasColumnName("id");

            entity.Property(x => x.Numero)
                .HasColumnName("numero");

            entity.Property(x => x.JugadorId)
                .HasColumnName("jugador_id");

            entity.Property(x => x.EquipoId)
                .HasColumnName("equipo_id");

            entity.Property(x => x.EdicionId)
                .HasColumnName("edicion_id");

            entity.Property(x => x.PublicacionId)
                .HasColumnName("publicacion_id");

            entity.Property(x => x.TipoCromoId)
                .HasColumnName("tipo_cromo_id");

            entity.Property(x => x.ColeccionId)
                .HasColumnName("coleccion_id");

            entity.Property(x => x.AlbumId)
                .HasColumnName("album_id");

            entity.HasOne(x => x.Jugador)
                .WithMany(x => x.Cromos)
                .HasForeignKey(x => x.JugadorId);

            entity.HasOne(x => x.Equipo)
                .WithMany(x => x.Cromos)
                .HasForeignKey(x => x.EquipoId);

            entity.HasOne(x => x.Edicion)
                .WithMany(x => x.Cromos)
                .HasForeignKey(x => x.EdicionId);

            entity.HasOne(x => x.Publicacion)
                .WithMany(x => x.Cromos)
                .HasForeignKey(x => x.PublicacionId);

            entity.HasOne(x => x.TipoCromo)
                .WithMany(x => x.Cromos)
                .HasForeignKey(x => x.TipoCromoId);

            entity.HasOne(x => x.Coleccion)
                .WithMany(x => x.Cromos)
                .HasForeignKey(x => x.ColeccionId);

            entity.HasOne(x => x.Album)
                .WithMany(x => x.Cromos)
                .HasForeignKey(x => x.AlbumId);
        });

        // EDITORIAL
        modelBuilder.Entity<Editorial>(entity =>
        {
            entity.ToTable("editorial");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Id)
                .HasColumnName("id");

            entity.Property(x => x.Nombre)
                .HasColumnName("nombre");
        });

        // ALBUM
        modelBuilder.Entity<Album>(entity =>
        {
            entity.ToTable("album");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Id)
                .HasColumnName("id");

            entity.Property(x => x.Nombre)
                .HasColumnName("nombre");

            entity.Property(x => x.Temporada)
                .HasColumnName("temporada");

            entity.Property(x => x.EditorialId)
                .HasColumnName("editorial_id");

            entity.HasOne(x => x.Editorial)
                .WithMany(x => x.Albums)
                .HasForeignKey(x => x.EditorialId);
        });

        // ALBUM_EQUIPO
        modelBuilder.Entity<AlbumEquipo>(entity =>
        {
            entity.ToTable("album_equipo");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Id)
                .HasColumnName("id");

            entity.Property(x => x.AlbumId)
                .HasColumnName("album_id");

            entity.Property(x => x.EquipoId)
                .HasColumnName("equipo_id");

            entity.Property(x => x.Orden)
                .HasColumnName("orden");

            entity.HasOne(x => x.Album)
                .WithMany(x => x.AlbumEquipos)
                .HasForeignKey(x => x.AlbumId);

            entity.HasOne(x => x.Equipo)
                .WithMany()
                .HasForeignKey(x => x.EquipoId);
        });

        // ALBUM_TIPO_CROMO
        modelBuilder.Entity<AlbumTipoCromo>(entity =>
        {
            entity.ToTable("album_tipo_cromo");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Id)
                .HasColumnName("id");

            entity.Property(x => x.AlbumId)
                .HasColumnName("album_id");

            entity.Property(x => x.TipoCromoId)
                .HasColumnName("tipo_cromo_id");

            entity.Property(x => x.Orden)
                .HasColumnName("orden");

            entity.HasOne(x => x.Album)
                .WithMany(x => x.AlbumTiposCromo)
                .HasForeignKey(x => x.AlbumId);

            entity.HasOne(x => x.TipoCromo)
                .WithMany()
                .HasForeignKey(x => x.TipoCromoId);
        });

        // USUARIO_CROMO
        modelBuilder.Entity<UsuarioCromo>(entity =>
        {
            entity.ToTable("usuario_cromo");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Id)
                .HasColumnName("id");

            entity.Property(x => x.UsuarioId)
                .HasColumnName("usuario_id");

            entity.Property(x => x.CromoId)
                .HasColumnName("cromo_id");

            entity.HasOne(x => x.Usuario)
                .WithMany(x => x.Cromos)
                .HasForeignKey(x => x.UsuarioId);

            entity.HasOne(x => x.Cromo)
                .WithMany(x => x.UsuariosCromo)
                .HasForeignKey(x => x.CromoId);
        });
    }
}