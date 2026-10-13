# SISMA - Student Registration App (WPF Rich UI & MVVM)

Aplikasi desktop registrasi dan manajemen mahasiswa profesional berbasis **WPF (Windows Presentation Foundation)** dengan arsitektur **MVVM (Model-View-ViewModel)** dan integrasi **Database SQL Server (StudentRegistrationDB)** sesuai dengan spesifikasi pada dokumentasi dan diagram relasi.

---

## 📸 Ringkasan Fitur Sesuai 4 Level (Roadmap)

### 1. Level 1 — Basic UI
- **Form Registrasi Mahasiswa**:
  - Input field lengkap:
    - **Nama Mahasiswa** (TextBox)
    - **NIM** (TextBox, validasi unik)
    - **Program Studi** (ComboBox dengan data relasi tabel `Programs`)
    - **Tanggal Lahir** (DatePicker)
    - **Alamat** (TextBox)
    - **No. Telepon** (TextBox)
  - Tombol aksi: **Simpan** dan **Reset**.
  - Validasi input dasar sebelum penyimpanan.

### 2. Level 2 — CRUD (Create, Read, Update, Delete)
- **DataGrid**: Menampilkan data mahasiswa secara rapi dan terstruktur.
- **Pencarian (Search)**: Pencarian interaktif berdasarkan Nama, NIM, atau Program Studi.
- **Edit & Hapus per Baris**: Tombol ikon aksi langsung di setiap baris tabel.
- **Konfirmasi Hapus**: Modal dialog konfirmasi dengan ikon peringatan sebelum menghapus data:
  > *"Apakah Anda yakin ingin menghapus data mahasiswa ini?"*
  > Tombol: **[Ya, Hapus]** dan **[Batal]**.
- **Counter Total Mahasiswa**: Menampilkan jumlah mahasiswa aktif secara real-time.
- **Navigasi Pagination**: Kontrol halaman tabel.

### 3. Level 3 — Rich UI (SISMA)
- **Nama Aplikasi**: **SISMA** (*Sistem Informasi Mahasiswa*).
- **Sidebar Navigation**:
  - Menu: **Dashboard**, **Mahasiswa**, **Mata Kuliah**, **Laporan**, **Pengaturan**.
  - Brand header dengan ikon toga (*graduation cap*).
  - Profil pengguna: **Admin** (`admin@kampus.ac.id`).
- **Dark Mode / Light Mode**:
  - Toggle switch modern untuk beralih antara tema gelap (slate/navy premium) dan tema terang.
- **Header Atas**:
  - Quick Search bar (*"Cari mahasiswa atau NIM..."*).
  - Ikon notifikasi dengan badge indikator.
  - Indikator status koneksi database.
- **Dashboard Stat Cards**:
  1. **Total Mahasiswa** (Aksen Biru `#1E40AF`)
  2. **Program Studi** (Aksen Teal/Hijau `#065F46`)
  3. **Dosen** (Aksen Oranye `#9A3412`)
  4. **Angkatan** (Aksen Ungu `#5B21B6`)
- **Grafik Mahasiswa**:
  - Visualisasi grafik batang (*Bar Chart*) interaktif menampilkan sebaran jumlah mahasiswa per program studi (**TI, SI, MNJ, TE, DKV**).
- **Aktivitas Terbaru**:
  - Riwayat linimasa penambahan, pembaruan, dan penghapusan data secara *real-time*.

### 4. Level 4 — Professional UI & Arsitektur
- **Pola Arsitektur MVVM**:
  - **View (XAML)**: Tampilan visual deklaratif, gaya modern, respon visual hover dan klik.
  - **ViewModel**: `MainViewModel`, `ViewModelBase` (`INotifyPropertyChanged`), `RelayCommand` & `AsyncRelayCommand` (`ICommand`).
  - **Model**: `Student`, `ProgramItem`, `ProgramStatistic`, `ActivityLog`.
- **Repository Pattern**:
  - Interface: `IStudentRepository`
  - Implementasi: `StudentRepository` menggunakan **ADO.NET (`Microsoft.Data.SqlClient`)** dan fallback otomatis **SQLite** lokal agar dapat langsung dijalankan tanpa setup SQL Server tambahan jika diperlukan.
- **Asynchronous (`Async / Await`)**: Semua operasi I/O database berjalan secara asinkron tanpa memblokir UI thread.

---

## 🗄️ Struktur Database SQL Server (StudentRegistrationDB)

File SQL lengkap: [`StudentRegistrationDB.sql`](./StudentRegistrationDB.sql).

### 1. Relasi Tabel (1 : N / One-to-Many)
- Tabel **`Programs`** (Program Studi):
  - `ProgramId` (PK, INT IDENTITY)
  - `ProgramCode` (VARCHAR(20), UNIQUE)
  - `ProgramName` (VARCHAR(100))
  - `IsActive` (BIT, DEFAULT 1)
  - `CreatedAt` (DATETIME2, DEFAULT SYSDATETIME())
- Tabel **`Students`** (Mahasiswa):
  - `StudentId` (PK, INT IDENTITY)
  - `NIM` (VARCHAR(20), UNIQUE)
  - `Name` (VARCHAR(100))
  - `ProgramId` (FK -> `Programs.ProgramId`)
  - `BirthDate` (DATE)
  - `Address` (VARCHAR(250))
  - `PhoneNumber` (VARCHAR(20))
  - `CreatedAt` (DATETIME2, DEFAULT SYSDATETIME())
  - `UpdatedAt` (DATETIME2)

### 2. Data Awal Program Studi
1. `TI` - Teknik Informatika
2. `SI` - Sistem Informasi
3. `MNJ` - Manajemen
4. `TE` - Teknik Elektro
5. `DKV` - Desain Komunikasi Visual

### 3. Data Contoh Mahasiswa (5 Mahasiswa Pertama)
1. `20231001` - Ahmad Rizki (TI)
2. `20231002` - Siti Aisyah (SI)
3. `20231003` - Budi Santoso (TI)
4. `20231004` - Citra Lestari (MNJ)
5. `20231005` - Dimas Prayoga (TE)

### 4. Query Standar
Sesuai Gambar 1:
- **Query 6**: Menampilkan Data Mahasiswa (`INNER JOIN Programs`)
- **Query 7**: Pencarian Mahasiswa (`LIKE %...%`)
- **Query 8**: Jumlah Mahasiswa (Total & Per Program Studi)
- **Query 9**: Edit dan Hapus Mahasiswa

---

## 🚀 Cara Menjalankan Proyek

### Menggunakan Visual Studio
1. Buka file solusi: `StudentRegistrationApp.sln`.
2. Pastikan target startup project adalah `StudentRegistrationApp`.
3. Tekan **F5** atau klik tombol **Start**.

### Menggunakan Command Line (.NET CLI)
```powershell
cd g:\Data-Pend1X\Code\PBKK\StudentRegistrationApp\StudentRegistrationApp
dotnet run
```
