# 🏫 University Classroom Booking Management System (UCBMS)

A web-based **classroom reservation and approval system** developed for **FPT University**, built with **.NET Framework (C#)** and **SQL Server**.
The system allows **students** and **lecturers** to submit room booking requests for study sessions, group projects, or teaching purposes, while **staff administrators** manage, approve, and monitor all room usage.

---

## 🎯 Features & Objectives

* Simplify the **room booking and approval process** at university.
* Ensure **efficient classroom allocation** and avoid scheduling conflicts.
* Provide transparency and traceability in booking history.
* Manage classroom information, capacity, and maintenance status.

---

## ⚙️ Core Modules

| Module                     | Description                                                             |
| -------------------------- | ----------------------------------------------------------------------- |
| 🧑‍🎓 **Student**          | Create and track room requests, select participants for study sessions. |
| 👨‍🏫 **Lecturer**         | Request classrooms for teaching, seminars, or project meetings.         |
| 👩‍💼 **Staff (Admin)**    | Approve or reject room requests, manage rooms, time slots, and users.   |
| 🏠 **Room Management**     | Add, edit, and track room capacity and availability.                    |
| ⏰ **Time Slot Management** | Define learning time slots (e.g., 7:30–9:00, 9:10–10:40).               |
| 📅 **Booking Management**  | View all approved bookings and usage reports.                           |

---

## 💻 Technologies Used

| Category            | Stack                                                             |
| ------------------- | ----------------------------------------------------------------- |
| **Language**        | C# (.NET Framework)                                               |
| **Frontend**        | Windows Presentation Foundation                                   |
| **Backend**         | .NET Framework, Entity Framework                                  |
| **Database**        | SQL Server                                                        |
| **Tools**           | Visual Studio, SSMS                                               |
| **Version Control** | Git + GitHub                                                      |

---

## 🧠 System Roles

| Role              | Description                                  |
| ----------------- | -------------------------------------------- |
| **Student**       | Send room requests and view approval status. |
| **Lecturer**      | Send room requests for academic purposes.    |
| **Staff (Admin)** | Manage room booking requests, rooms, slots, and all users. |

---

## 🗂 Database Overview

Key tables:

* `User` → Base login and role management
* `StudentProfile`, `LecturerProfile`, `StaffProfile`
* `Room`, `TimeSlot`
* `RoomRequest`, `RoomRequest_Participant`, `Booking`

---

## 🚀 How to Run

1. Clone the repository

   ```bash
   git clone https://github.com/iambuli/University-Classroom-Booking-Management-System.git
   ```
2. Open the `.sln` file in **Visual Studio**
3. Configure **SQL Server connection string** in `App.config` or `web.config`
4. Run the database script `database.sql`
5. Build and start the project

---

## 🧑‍💻 Author

**Hoàng Quân** – FPT University
📧 Contact: [hoanganhquan.work@gmail.com](mailto:hoanganhquan.work@gmail.com)

---
