 <%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Index.aspx.cs" Inherits="SMSWEBAPP.Index" %>

<!DOCTYPE html>
<script runat="server">

</script>

<html lang="en">

<head runat="server">
  <meta charset="utf-8">
  <meta content="width=device-width, initial-scale=1.0" name="viewport">
  <title>Fecomas Tech Solutions</title>
  <meta name="description" content="">
  <meta name="keywords" content="">

  <!-- Favicons -->
  <link href="assets/img/FTSlogo.png" rel="icon">
  <link href="assets/img/FTSlogo.png" rel="apple-touch-icon"> <!-- Corrected path -->

  <!-- Fonts -->
  <link href="https://fonts.googleapis.com" rel="preconnect">
  <link href="https://fonts.gstatic.com" rel="preconnect" crossorigin>
  <link href="https://fonts.googleapis.com/css2?family=Roboto:ital,wght@0,100;0,300;0,400;0,500;0,700;0,900;1,100;1,300;1,400;1,500;1,700;1,900&family=Poppins:ital,wght@0,100;0,200;0,300;0,400;0,500;0,600;0,700;0,800;0,900;1,100;1,200;1,300;1,400;1,500;1,600;1,700;1,800;1,900&family=Raleway:ital,wght@0,100;0,200;0,300;0,400;0,500;0,600;0,700;0,800;0,900;1,100;1,200;1,300;1,400;1,500;1,600;1,700;1,800;1,900&display=swap" rel="stylesheet">

  <!-- Vendor CSS Files -->
  <link href="assets/vendor/bootstrap/css/bootstrap.min.css" rel="stylesheet">
  <link href="assets/vendor/bootstrap-icons/bootstrap-icons.css" rel="stylesheet">
  <link href="assets/vendor/aos/aos.css" rel="stylesheet">
  <link href="assets/vendor/swiper/swiper-bundle.min.css" rel="stylesheet">
  <link href="assets/vendor/glightbox/css/glightbox.min.css" rel="stylesheet">

  <!-- Main CSS File -->
  <link href="assets/css/main.css" rel="stylesheet">

  <!-- Ensure jQuery is included for WebForms Unobtrusive Validation -->
  <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
</head>

<body class="index-page">

<header id="header" class="header d-flex align-items-center fixed-top">
    <div class="container-fluid container-xl position-relative d-flex align-items-center justify-content-between">

        <a href="index.aspx" class="logo d-flex align-items-center">
            <!-- Uncomment the line below if you also wish to use an image logo -->
            <!--<img src="assets/img/FTSLOGO.png" alt="Fecomas Logo" class="logo-circle">-->
            <div>
                <h1 class="sitename">Fecomas School Management System</h1>
                <p class="slogan">Automate, Simplify, Make your school Shine</p>
            </div>
        </a>

<nav id="navmenu" class="navmenu">
    <ul>
        <li><a href="#hero" class="active"><i class="bi bi-house-door"></i> Home</a></li>
        <li><a href="#features"><i class="bi bi-briefcase"></i> Features</a></li>
        <li><a href="#team"><i class="bi bi-person-fill"></i> Team</a></li>
<li><a href="https://wa.me/265993189671" target="_blank"><i class="bi bi-whatsapp"></i> Book Demo</a></li>
    </ul>
    <i class="mobile-nav-toggle d-xl-none bi bi-list"></i>
</nav>

    </div>
</header>
    <a href="https://wa.me/265993189671" class="whatsapp-container" target="_blank">
        <div class="whatsapp-float">
            <i class="bi bi-whatsapp"></i>
        </div>
        <i class="bi bi-hand-index-thumb hand-icon"></i>
    </a> 
    <main class="main">
        <!-- Hero Section -->
<style>

    .navmenu ul li a {
    font-size: 18px; /* adjust the font size as needed */
}
    .navmenu ul li a[href*="whatsapp"] {
    background-color: #25D366; /* WhatsApp green color */
    color: #fff; /* White text color */
}
.whatsapp-container {
    position: fixed;
    bottom: 60px;
    right: 20px;
    display: flex;
    flex-direction: column;
    align-items: center;
    text-decoration: none;
    gap: 5px;
}

.whatsapp-float {
    background-color: #25d366;
    color: white;
    padding: 12px;
    font-size: 24px;
    border-radius: 50px;
    text-align: center;
    box-shadow: 2px 2px 10px rgba(0, 0, 0, 0.2);
    transition: background-color 0.3s ease;
}

.whatsapp-float:hover {
    background-color: #1ebe5d;
}

.whatsapp-float i {
    font-size: 28px;
}

.hand-icon {
    font-size: 30px;
    color: #25d366;
    animation: bounce 1.5s infinite alternate;
}

@keyframes bounce {
    0% { transform: translateY(0); }
    100% { transform: translateY(8px); }
}

.section-title p {
    font-weight: normal;
    font-size: 16px;
    margin-top: 0;
    margin-bottom: 5px;
}

.team-member .member-img img {
    width: 100%;
    height: 250px;
    object-fit: cover;
    border-radius: 30px;
}

.clients {
    position: relative;
}

.watermark-background {
    position: absolute;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    display: flex;
    justify-content: center;
    align-items: center;
    opacity: 0.1;
    z-index: 0;
}

.watermark-background h2 {
    font-size: 4rem;
    color: #000;
    font-weight: bold;
    transform: rotate(-45deg);
    text-transform: uppercase;
    letter-spacing: 10px;
}

.logo-slider {
    position: relative;
    z-index: 1;
    overflow: hidden;
    width: 100%;
}

.logo-track {
    display: flex;
    width: fit-content;
    animation: slide 20s linear infinite;
}

@keyframes slide {
    0% {
        transform: translateX(0);
    }
    100% {
        transform: translateX(-50%);
    }
}

.member-img img {
    border-radius: 50%;
    object-fit: cover;
}

.book-demo-btn {
    background-color: #0B0F1A;
    color: #fff;
    border-radius: 30px;
    padding: 10px 20px;
    font-weight: bold;
    transition: all 0.3s ease-in-out;
    border: none;
    text-align: center;
    display: inline-block;
}

.book-demo-btn:hover {
    background-color: #141A2E;
    transform: scale(1.05);
}

#clients.section {
    padding-top: 0 !important;
    padding-bottom: 0 !important;
    margin-top: 0 !important;
    margin-bottom: 0 !important;
}

.slogan {
    font-size: 14px;
    color: #ddd;
    font-style: italic;
    margin-top: -5px;
}

.hero.section {
    padding-bottom: 0 !important;
    margin-bottom: 0 !important;

}


.logo-slider {
    overflow: hidden;
    width: 100%;
}

.logo-track {
    display: flex;
    flex-wrap: nowrap;
    animation: slide 60s linear infinite;
}

.client-logo {
    margin: 0 20px;
    text-align: center;
    flex-shrink: 0;
}

.client-logo img {
    height: 150px;
    width: auto;
    object-fit: contain;
    margin-bottom: 10px;
}

.client-logo p {
    font-size: 16px;
    font-weight: bold;
}

@keyframes slide {
    0% {
        transform: translateX(0);
    }
    100% {
        transform: translateX(-50%);
    }
}

#clients {
    background-size: cover;
    background-position: center;
    position: relative;
}

#clients::before {
    content: '';
    position: absolute;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    background-color: rgba(255, 255, 255, 0.9); /* adjust opacity here */
    z-index: -1;
}

#clients .container-fluid {
    position: relative;
    z-index: 1;
}

.logo-top-left {
    position: absolute;
    top: 20px;
    left: 20px;
    z-index: 1;
}

.logo-circle {
    width: 100px;
    height: 100px;
    border-radius: 50%;
    object-fit: cover;
    border: 2px solid #fff; /* Add a white border */
    box-shadow: 0 0 10px rgba(0, 0, 0, 0.2); /* Add a subtle shadow */
}

#clients {
    margin-top: -50px; /* adjust the value as needed */
}
#hero {
    padding-bottom: 0;
}
.section-padding {
    margin-top: 50px; /* adjust the value as needed */
}
@media (max-width: 768px) {
  #hero h1 {
    margin-top: 60px; /* adjust the value as needed */
  }
}
</style>
<section id="hero" class="hero section dark-background pt-5">   
    <img src="assets/img/hero-bg4.jpg" alt="" data-aos="fade-in">

    <div class="logo-top-left">
        <img src="assets/img/FTSLOGO.png" alt="Fecomas Logo" class="logo-circle">
    </div>

    <div class="container text-center" data-aos="fade-up" data-aos-delay="100">

<div style="margin-bottom: 8px; margin-top: 16px;">
    <a href="https://fecomas.com/views/Admin/UserLogin.aspx" class="btn btn-dark btn-lg px-4 shadow login-btn">
        <i class="bi bi-box-arrow-in-right"></i> Login
    </a>
</div>

<h1 class="fw-bold" 
    style="margin-bottom:5px; color:#032B44; text-shadow:1px 1px 3px rgba(255,255,255,0.7);">
  One - Stop Software for Managing all School Operations
</h1>

        <div class="row justify-content-center">
            <div class="col-lg-12">

                <!-- Hero Carousel -->
                <div id="hero-carousel" class="carousel slide" data-bs-ride="carousel" data-bs-interval="4000">
                    <div class="carousel-inner">

                        <!-- 1️⃣ Student Registration -->
                        <div class="carousel-item active">
                            <h2 class="fw-bold">
                                <i class="bi bi-person-plus"></i> Student Registration & Enrollment <i class="bi bi-person-plus"></i>
                            </h2>
                            <p class="lead">
                                Effortlessly manage student registrations, admissions, and enrollment with our intuitive system. Easily track student data, documents, and status.
                            </p>
                        </div>

                        <!-- 2️⃣ Invoice Management -->
                        <div class="carousel-item">
                            <h2 class="fw-bold">
                                <i class="bi bi-receipt"></i> Invoice Management <i class="bi bi-receipt"></i>
                            </h2>
                            <p class="lead">
                                Streamline your billing process with our automated invoice management system. Generate invoices, track payments, and manage receipts with ease.
                            </p>
                        </div>

                        <!-- 3️⃣ Fees Collection Management -->
                        <div class="carousel-item">
                            <h2 class="fw-bold">
                                <i class="bi bi-cash-coin"></i> Fees Collection Management <i class="bi bi-cash-coin"></i>
                            </h2>
                            <p class="lead">
                                Simplify fees collection and tracking with our system. Automate payment reminders, track payments, and generate receipts.
                            </p>
                        </div>

                        <!-- 4️⃣ Examination Management -->
                        <div class="carousel-item">
                            <h2 class="fw-bold">
                                <i class="bi bi-pencil-square"></i> Examination Management <i class="bi bi-pencil-square"></i>
                            </h2>
                            <p class="lead">
                                Effortlessly manage exams, assessments, and results with our system. Automatically generate school reports and track student performance.
                            </p>
                        </div>

                        <!-- 5️⃣ Library Management -->
                        <div class="carousel-item">
                            <h2 class="fw-bold">
                                <i class="bi bi-book"></i> Library Management <i class="bi bi-book"></i>
                            </h2>
                            <p class="lead">
                                Streamline library operations with our system. Manage book inventory, track borrowing, and generate reports.
                            </p>
                        </div>

                        <!-- 6️⃣ Assets Management -->
                        <div class="carousel-item">
                            <h2 class="fw-bold">
                                <i class="bi bi-box-seam"></i> Assets Management <i class="bi bi-box-seam"></i>
                            </h2>
                            <p class="lead">
                                Effectively manage school assets, track inventory, and generate reports with our system.
                            </p>
                        </div>

                        <!-- 7️⃣ Accounting -->
                        <div class="carousel-item">
                            <h2 class="fw-bold">
                                <i class="bi bi-calculator"></i> Accounting <i class="bi bi-calculator"></i>
                            </h2>
                            <p class="lead">
                                Manage income, expenses, and budgeting with our comprehensive accounting system. Generate financial reports and track school finances.
                            </p>
                        </div>

                        <!-- 8️⃣ Reports Generation -->
                        <div class="carousel-item">
                            <h2 class="fw-bold">
                                <i class="bi bi-graph-up"></i> Reports Generation <i class="bi bi-graph-up"></i>
                            </h2>
                            <p class="lead">
                                Generate detailed reports on student enrollment, fees collection, library inventory, student academic progress, and more. Make informed decisions with data-driven insights.
                            </p>
                        </div>

                        <!-- 9️⃣ Student Academic Progress Reports -->
                        <div class="carousel-item">
                            <h2 class="fw-bold">
                                <i class="bi bi-mortarboard"></i> Student Academic Progress Reports <i class="bi bi-mortarboard"></i>
                            </h2>
                            <p class="lead">
                                Track student performance and progress with our comprehensive reporting system. Generate reports on student grades, attendance, and more.
                            </p>
                        </div>
                    </div>

                    <!-- Carousel Controls -->
                    <button class="carousel-control-prev" type="button" data-bs-target="#hero-carousel" data-bs-slide="prev">
                        <span class="carousel-control-prev-icon" aria-hidden="true"></span>
                        <span class="visually-hidden">Previous</span>
                    </button>
                    <button class="carousel-control-next" type="button" data-bs-target="#hero-carousel" data-bs-slide="next">
                        <span class="carousel-control-next-icon" aria-hidden="true"></span>
                        <span class="visually-hidden">Next</span>
                    </button>
                </div>

                <!-- Bottom buttons -->
                <div class="row mt-4">
                    <div class="col-md-6 text-center mb-3">
                        <a href="#features" class="btn" style="background-color: #032B44; color: #fff;">
                            <i class="bi bi-youtube" style="color: red;"></i> Watch Video Tutorials on YouTube
                        </a>
                    </div>
                    <div class="col-md-6 text-center mb-3">
                        <a href="https://fecomas.com/Guide/UserGuideSchoolManagementSystem.pdf" target="_blank" class="btn btn-secondary" download>
                            <i class="bi bi-file-earmark-pdf" style="color: red;"></i> Download Complete User Guide
                        </a>
                    </div>
                </div>

            </div>
        </div>
    </div>
</section>

<style>
/* ✅ Mobile-specific fine-tuned fix */
@media (max-width: 768px) {
    #hero .login-btn {
        margin-top: 70px !important; /* reduced from 70px to 30px */
        font-size: 1rem;
        padding: 10px 24px;
    }
}

</style>
        <!-- Clients Section -->
<section id="clients" class="py-5" style="background-image: url('assets/img/Hero-bg.jpg'); background-size: cover; background-position: center; background-repeat: no-repeat; background-attachment: fixed; background-blend-mode: overlay; background-color: rgba(255, 255, 255, 0.7);">
    <div class="text-center">
        <h2 class="d-inline-block me-2">Our Clients</h2>
        <p class="d-inline-block fs-1 fw-bold" id="client-count">0</p>
    </div>

    <div class="container-fluid px-0">
        <div class="logo-slider">
            <div class="logo-track">
                <div class="client-logo">
                    <p class="mb-0">Blue Bird High School</p>
                    <img src="assets/img/clients/BBHS.png" alt="BBHS" class="img-fluid">
                </div>
                <div class="client-logo">
                    <p class="mb-0">BFPS</p>
                    <img src="assets/img/clients/BFPS.png" alt="BFPS" class="img-fluid">
                </div>

                <div class="client-logo">
                    <p class="mb-0">Jampark Secondary School</p>
                    <img src="assets/img/clients/jmk.png" alt="BFPS" class="img-fluid">
                </div>

                <div class="client-logo">
                    <p class="mb-0">Good Soil Academy</p>
                    <img src="assets/img/clients/GSA.png" alt="BFPS" class="img-fluid">
                </div>
                <div class="client-logo">
                    <p class="mb-0">Jamo Private School</p>
                    <img src="assets/img/clients/Jamo.png" alt="BFPS" class="img-fluid">
                </div>
                <div class="client-logo">
                    <p class="mb-0">Tafika Tione Schools</p>
                    <img src="assets/img/clients/tafika.png" alt="BFPS" class="img-fluid">
                </div>

                <div class="client-logo">
                    <p class="mb-0">Tiwale Private School</p>
                    <img src="assets/img/clients/Tiwale.jpg" alt="BFPS" class="img-fluid">
                </div>

                <div class="client-logo">
                    <p class="mb-0">High Achievers Schools</p>
                    <img src="assets/img/clients/Hass.jpg" alt="BFPS" class="img-fluid">
                </div>

                <div class="client-logo">
                    <p class="mb-0">MDF Secondary School</p>
                    <img src="assets/img/clients/mdf.jpg" alt="BFPS" class="img-fluid">
                </div>

                <div class="client-logo">
                    <p class="mb-0">Sosten Gwengwe Foundation</p>
                    <img src="assets/img/clients/sgf.png" alt="BFPS" class="img-fluid">
                </div>

                <div class="client-logo">
                    <p class="mb-0">Alice Gwengwe Foundation</p>
                    <img src="assets/img/clients/agf.png" alt="BFPS" class="img-fluid">
                </div>

                <div class="client-logo">
                    <p class="mb-0">Izimvale Academy</p>
                    <img src="assets/img/clients/Izimvale.png" alt="BFPS" class="img-fluid">
                </div>

                <div class="client-logo">
                    <p class="mb-0">Peniel Foundation</p>
                    <img src="assets/img/clients/peniel.png" alt="BFPS" class="img-fluid">
                </div>
                <div class="client-logo">
                    <p class="mb-0">VinielFoundation Academy</p>
                    <img src="assets/img/clients/vlf.jpg" alt="BFPS" class="img-fluid">
                </div>

                <div class="client-logo">
                    <p class="mb-0">New Model Foundation</p>
                    <img src="assets/img/clients/NEMA.png" alt="BFPS" class="img-fluid">
                </div>

                <div class="client-logo">
                    <p class="mb-0">CARDINAL C.L ACADEMY</p>
                    <img src="assets/img/clients/CCL.png" alt="CCL" class="img-fluid">
                </div>
                <div class="client-logo">
                    <p class="mb-0">Chiedza Primary School</p>
                    <img src="assets/img/clients/CPS.JPG" alt="CPS" class="img-fluid">
                </div>
                <div class="client-logo">
                    <p class="mb-0">Dzuka Girls Private </p>
                    <img src="assets/img/clients/DKG.PNG" alt="DKG" class="img-fluid">
                </div>
                <div class="client-logo">
                    <p class="mb-0">Dzuka Private Academy</p>
                    <img src="assets/img/clients/DPA.png" alt="DPA" class="img-fluid">
                </div>
                <div class="client-logo">
                    <p class="mb-0">Gray matter Academy</p>
                    <img src="assets/img/clients/GMA.jpg" alt="GMA" class="img-fluid">
                </div>
                <div class="client-logo">
                    <p class="mb-0">Mzimba Christian Academy</p>
                    <img src="assets/img/clients/MCA.jpg" alt="SAN" class="img-fluid">
                </div>
                <div class="client-logo">
                    <p class="mb-0">Glory Pvt Academy</p>
                    <img src="assets/img/clients/Gps.jpg" alt="SAN" class="img-fluid">
                </div>
                <div class="client-logo">
                    <p class="mb-0">Elephant Lock Academy</p>
                    <img src="assets/img/clients/Elephant.jpg" alt="SAN" class="img-fluid">
                </div>
                <div class="client-logo">
                    <p class="mb-0">Minjiru View Schools</p>
                    <img src="assets/img/clients/Minjiru.png" alt="SAN" class="img-fluid">
                </div>
            </div>
        </div>
    </div>
</section>        <!-- Services Section -->
<!-- Add this in the <head> section of your HTML for the Century Gothic font -->
<style>
    body {
        font-family: 'Century Gothic', sans-serif;


    }

    .custom-btn {
    background-color: #000033 !important; /* Very Dark Blue */
    color: white !important; /* White Foreground */
    border: none;
    padding: 10px 20px;
    font-size: 16px;
}

.custom-btn:hover {
    background-color: #000066 !important; /* Slightly Lighter Blue on Hover */
}

</style>

<section id="features" class="services section" 
    style="background-image: url('assets/img/hero-bg3.jpg'); 
           background-size: cover; 
           background-position: center; 
           background-repeat: no-repeat; 
           background-attachment: fixed; 
           background-blend-mode: overlay; 
           background-color: rgba(255, 255, 255, 0.9);">

    <!-- Section Title -->
    <div class="container section-title text-center" data-aos="fade-up">
        <h2 style="font-family: 'Century Gothic', sans-serif; font-weight: bold; color: #001f3d;">
            Fecomas School Management System Features
        </h2>
        <p>Streamline your school operations with our comprehensive and integrated system.</p>
    </div>

    <!-- Features Grid -->
    <div class="container">
        <div class="row gy-5">
                        <!-- 2. System Overview -->
            <div class="col-lg-6 col-md-6" data-aos="fade-up" data-aos-delay="200">
                <div class="card shadow-sm border-0 h-100 text-center">
                    <iframe width="100%" height="250" src="https://www.youtube.com/embed/ndOUCfWLNWQ"
                        title="Fecomas Overview" frameborder="0"
                        allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture"
                        allowfullscreen></iframe>
                    <div class="card-body">
                        <div class="icon mb-3"><i class="bi bi-info-circle display-6"></i></div>
                        <h5 class="card-title fw-bold" style="font-family:'Century Gothic';color:#001f3d;">
                            Fecomas School Management System Overview
                        </h5>
                        <p>Get a comprehensive look at how Fecomas helps your school run efficiently through automation and analytics.</p>
                    </div>
                </div>
            </div>

            <!-- 1. Student Registration -->
            <div class="col-lg-6 col-md-6" data-aos="fade-up" data-aos-delay="100">
                <div class="card shadow-sm border-0 h-100 text-center">
<iframe width="100%" height="250" src="https://www.youtube.com/embed/XJLXZ1Fmx7g?si=VHso_5urHTUY8OBj" title="YouTube video player" frameborder="0" allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture; web-share" referrerpolicy="strict-origin-when-cross-origin" allowfullscreen></iframe>
                    <div class="card-body">
                        <div class="icon mb-3"><i class="bi bi-person-plus display-6"></i></div>
                        <h5 class="card-title fw-bold" style="font-family:'Century Gothic';color:#001f3d;">
                            Student Registration & Enrollment
                        </h5>
                        <p>Effortlessly manage student registrations, admissions, and enrollment with our intuitive system.</p>
                    </div>
                </div>
            </div>


            <!-- 3. Fees Collection -->
            <div class="col-lg-6 col-md-6" data-aos="fade-up" data-aos-delay="300">
                <div class="card shadow-sm border-0 h-100 text-center">
                    <iframe width="100%" height="250" src="https://www.youtube.com/embed/H23dEDQgv4I"
                        title="Fees Collection Management" frameborder="0"
                        allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture"
                        allowfullscreen></iframe>
                    <div class="card-body">
                        <div class="icon mb-3"><i class="bi bi-cash-coin display-6"></i></div>
                        <h5 class="card-title fw-bold" style="font-family:'Century Gothic';color:#001f3d;">
                            Fees Collection Management
                        </h5>
                        <p>Simplify fees collection and tracking with our integrated payment system and instant reporting.</p>
                    </div>
                </div>
            </div>

            <!-- 4. Examination Management -->
            <div class="col-lg-6 col-md-6" data-aos="fade-up" data-aos-delay="400">
                <div class="card shadow-sm border-0 h-100 text-center">
                    <iframe width="100%" height="250" src="https://www.youtube.com/embed/kQyhmWNvs_I"
                        title="Examination Management" frameborder="0"
                        allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture"
                        allowfullscreen></iframe>
                    <div class="card-body">
                        <div class="icon mb-3"><i class="bi bi-pencil-square display-6"></i></div>
                        <h5 class="card-title fw-bold" style="font-family:'Century Gothic';color:#001f3d;">
                            Exam Management And Automatatic School Reports Generation
                        </h5>
                        <p>Manage exams, assessments, and results efficiently with automated grading and result analytics.</p>
                    </div>
                </div>
            </div>

                        <!-- 4. Examination Management -->
            <div class="col-lg-6 col-md-6" data-aos="fade-up" data-aos-delay="400">
                <div class="card shadow-sm border-0 h-100 text-center">
<iframe width="100%" height="250" src="https://www.youtube.com/embed/erOuq8NAprY?si=AHS9A-HEG7AUgO6T" title="YouTube video player" frameborder="0" allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture; web-share" referrerpolicy="strict-origin-when-cross-origin" allowfullscreen></iframe>                    <div class="card-body">
                        <div class="icon mb-3"><i class="bi bi-pencil-square display-6"></i></div>
                        <h5 class="card-title fw-bold" style="font-family:'Century Gothic';color:#001f3d;">
                            How Can Parents/Students Check Exam Results 
                        </h5>
<p>To check your exam results, follow these steps:</p>
<ol>
    <li>Click the button for the exam report you want to check </li>
    <li>Select a Term in a dropdown list</li>
    <li>Report will load automatically</li>
    <li>You can Export the generated Report in pdf, word or Excel</li>
</ol>                    </div>
                </div>
            </div>

            <!-- 5. Library Management -->
            <div class="col-lg-6 col-md-6" data-aos="fade-up" data-aos-delay="500">
                <div class="card shadow-sm border-0 h-100 text-center">
                    <div class="ratio ratio-16x9 bg-light d-flex align-items-center justify-content-center">
                        <p class="fs-6 text-muted m-0">🎥 Tutorial on this module coming soon!</p>
                    </div>
                    <div class="card-body">
                        <div class="icon mb-3"><i class="bi bi-book display-6"></i></div>
                        <h5 class="card-title fw-bold" style="font-family:'Century Gothic';color:#001f3d;">Library Management</h5>
                        <p>Streamline library operations with efficient Books cataloging, borrowing, and return tracking tools.</p>
                    </div>
                </div>
            </div>

            <!-- 6. Asset Management -->
            <div class="col-lg-6 col-md-6" data-aos="fade-up" data-aos-delay="600">
                <div class="card shadow-sm border-0 h-100 text-center">
                    <div class="ratio ratio-16x9 bg-light d-flex align-items-center justify-content-center">
                        <p class="fs-6 text-muted m-0">🎥 Tutorial on this module coming soon!</p>
                    </div>
                    <div class="card-body">
                        <div class="icon mb-3"><i class="bi bi-box-seam display-6"></i></div>
                        <h5 class="card-title fw-bold" style="font-family:'Century Gothic';color:#001f3d;">Asset Management</h5>
                        <p>Track, manage, and maintain all school assets including furniture, computers, and equipment.</p>
                    </div>
                </div>
            </div>

            <!-- 7. Accounting & Financial Reports -->
            <div class="col-lg-6 col-md-6" data-aos="fade-up" data-aos-delay="700">
                <div class="card shadow-sm border-0 h-100 text-center">
                    <div class="ratio ratio-16x9 bg-light d-flex align-items-center justify-content-center">
                        <p class="fs-6 text-muted m-0">🎥 Tutorial on this module coming soon!</p>
                    </div>
                    <div class="card-body">
                        <div class="icon mb-3"><i class="bi bi-calculator display-6"></i></div>
                        <h5 class="card-title fw-bold" style="font-family:'Century Gothic';color:#001f3d;">Accounting & Financial Reports</h5>
                        <p>Automate financial records, generate reports, and track income, expenses and budget to improve school accountability.</p>
                    </div>
                </div>
            </div>

            <!-- 9. SMS & WhatsApp Notification Management -->

            <div class="col-lg-6 col-md-6" data-aos="fade-up" data-aos-delay="900">
                <div class="card shadow-sm border-0 h-100 text-center">
                    <iframe width="100%" height="250" src="https://www.youtube.com/embed/NwtsQIqV0Kw"
                        title="SMS & WhatsApp Notification Management" frameborder="0"
                        allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture"
                        allowfullscreen></iframe>
                    <div class="card-body">
                        <div class="icon mb-3"><i class="bi bi-chat-dots display-6"></i></div>
                        <h5 class="card-title fw-bold" style="font-family:'Century Gothic'; color:#001f3d;">
                            SMS & WhatsApp Notification Management
                        </h5>
                        <p>Send instant SMS and WhatsApp messages to parents, students, and staff for updates, alerts, and announcements.</p>
                    </div>
                </div>
            </div>


        </div>
    </div>
</section>
                                <!-- /Features Section --><!-- Call To Action Section -->
<section id="call-to-action" class="call-to-action section" style="background-image: url('assets/img/Hero-bg2.jpg'); background-size: cover; background-position: center; background-repeat: no-repeat; background-attachment: fixed; background-blend-mode: overlay; background-color: rgba(0, 31, 61, 0.8);">
    <div class="container">
        <div class="row justify-content-center" data-aos="zoom-in" data-aos-delay="100">
            <div class="col-xl-10">
                <div class="text-center">
                    <h3 style="color: white;">Get Started with Fecomas School Management System</h3>
                    <p style="color: white;">Streamline your school operations with our comprehensive and integrated system. Contact us today and see the difference.</p>
                    <a class="cta-btn" href="#contact" style="background-color: white; color: #001f3d;">Contact Us Now</a>
                </div>
            </div>
        </div>
    </div>
</section><!-- /Call To Action Section --><!-- Testimonials Section -->
<section id="testimonials" class="testimonials section" style="background-image: url('assets/img/Hero-bg3.jpg'); background-size: cover; background-position: center; background-repeat: no-repeat; background-attachment: fixed; background-blend-mode: overlay; background-color: rgba(255, 255, 255, 0.9);">

    <!-- Section Title -->
    <div class="container section-title" data-aos="fade-up">
        <h2 style="color: #001f3d;">What Our Clients Say</h2>
        <p style="color: #001f3d;">Our clients trust us to provide innovative solutions for their businesses. Here's what they have to say about our services:</p>
    </div><!-- End Section Title -->

    <div class="container" data-aos="fade-up" data-aos-delay="100">

        <div class="swiper init-swiper">
            <script type="application/json" class="swiper-config">
                {
                  "loop": true,
                  "speed": 600,
                  "autoplay": {
                    "delay": 5000
                  },
                  "slidesPerView": "auto",
                  "pagination": {
                    "el": ".swiper-pagination",
                    "type": "bullets",
                    "clickable": true
                  },
                  "breakpoints": {
                    "320": {
                      "slidesPerView": 1,
                      "spaceBetween": 40
                    },
                    "1200": {
                      "slidesPerView": 3,
                      "spaceBetween": 1
                    }
                  }
                }
            </script>
            <div class="swiper-wrapper">

<div class="swiper-slide">
    <div class="testimonial-item">
        <p>
            <i class="bi bi-quote quote-icon-left"></i>
            <span>"Fecomas School Management System has been a game-changer for our school. The attendance tracking, grade management, and parent communication features have saved us a lot of time and effort. Highly recommended!" - Principal, Blue Bird High School</span>
            <i class="bi bi-quote quote-icon-right"></i>
        </p>
        <img src="assets/img/clients/placeholder.jpeg" class="testimonial-img" alt="">
        <h3>The Principal, Blue Bird High School</h3>
       <!-- <h4>The Principal, Blue Bird High School</h4> -->
    </div>
</div>

<div class="swiper-slide">
    <div class="testimonial-item">
        <p>
            <i class="bi bi-quote quote-icon-left"></i>
            <span>"The Fecomas School Management System has been a lifesaver for our school. The system is user-friendly and has improved our school's efficiency. We are grateful for the support and service provided by the Fecomas team." - Director, BFPS</span>
            <i class="bi bi-quote quote-icon-right"></i>
        </p>
        <img src="assets/img/clients/placeholder.jpeg" class="testimonial-img" alt="">
        <h3>The Director, Bright Future Pvt School</h3>
    </div>
</div>

<div class="swiper-slide">
    <div class="testimonial-item">
        <p>
            <i class="bi bi-quote quote-icon-left"></i>
            <span>"Fecomas School Management System has exceeded our expectations. The system's features have streamlined our school's operations, and the support team is always available to assist us." - Principal, Jampark Secondary School</span>
            <i class="bi bi-quote quote-icon-right"></i>
        </p>
        <img src="assets/img/clients/placeholder.jpeg" class="testimonial-img" alt="">
        <h3>Mr. S. Samuel</h3>
        <h4>The Director, Jampark Secondary School</h4>
    </div>
</div>

<div class="swiper-slide">
    <div class="testimonial-item">
        <p>
            <i class="bi bi-quote quote-icon-left"></i>
            <span>"The Fecomas School Management System has been a valuable asset to our school. The system's features have improved our school's efficiency, and the team is always available to assist us." - Director, Good Soil Academy</span>
            <i class="bi bi-quote quote-icon-right"></i>
        </p>
        <img src="assets/img/clients/placeholder.jpeg" class="testimonial-img" alt="">
        <h3>Mr. Ishmael Mussa</h3>
        <h4>The Accountant, Good Soil Academy</h4>
    </div>
</div>

<div class="swiper-slide">
    <div class="testimonial-item">
        <p>
            <i class="bi bi-quote quote-icon-left"></i>
            <span>"Fecomas School Management System has been a game-changer for our school. The system's features have streamlined our school's operations, and the support team is always available to assist us." - Principal, Jamo Private School</span>
            <i class="bi bi-quote quote-icon-right"></i>
        </p>
        <img src="assets/img/clients/placeholder.jpeg" class="testimonial-img" alt="">
        <h3>Mr. James Light</h3>
        <h4>The Director, Jamo Private School</h4>
    </div>
</div>

<div class="swiper-slide">
    <div class="testimonial-item">
        <p>
            <i class="bi bi-quote quote-icon-left"></i>
            <span>"The Fecomas School Management System has been a valuable asset to our school. The system's features have improved our school's efficiency, and the team is always available to assist us." - Director, Tafika Tione Schools</span>
            <i class="bi bi-quote quote-icon-right"></i>
        </p>
        <img src="assets/img/clients/placeholder.jpeg" class="testimonial-img" alt="">
        <h3>Mr. Pasca Mulambya</h3>
        <h4>The Accountant, Tafika Tione Schools</h4>
    </div>
</div>

<div class="swiper-slide">
    <div class="testimonial-item">
        <p>
            <i class="bi bi-quote quote-icon-left"></i>
            <span>"Fecomas School Management System has been a lifesaver for our school. The system is user-friendly and has improved our school's efficiency. We are grateful for the support and service provided by the Fecomas team." - Director, Tiwale Private School</span>
            <i class="bi bi-quote quote-icon-right"></i>
        </p>
        <img src="assets/img/clients/placeholder.jpeg" class="testimonial-img" alt="">
        <h3>Mr. Kondwani Bartholomew</h3>
        <h4>The Director, Tiwale Private School</h4>
    </div>
</div>

<div class="swiper-slide">
    <div class="testimonial-item">
        <p>
            <i class="bi bi-quote quote-icon-left"></i>
            <span>"The Fecomas School Management System has exceeded our expectations. The system's features have streamlined our school's operations, and the support team is always available to assist us." - Principal, High Achievers Schools</span>
            <i class="bi bi-quote quote-icon-right"></i>
        </p>
        <img src="assets/img/clients/placeholder.jpeg" class="testimonial-img" alt="">
        <h3>Mr. Sam Silumbu</h3>
        <h4>The Principal, High Achievers Schools</h4>
    </div>
</div>

<div class="swiper-slide">
    <div class="testimonial-item">
        <p>
            <i class="bi bi-quote quote-icon-left"></i>
            <span>"Fecomas School Management System has been a valuable asset to our school. The system's features have improved our school's efficiency, and the team is always available to assist us." - Director, MDF Secondary School</span>
            <i class="bi bi-quote quote-icon-right"></i>
        </p>
        <img src="assets/img/clients/placeholder.jpeg" class="testimonial-img" alt="">
        <h3>Mr Hastings Lisuntha</h3>
        <h4>A Teacher, MDF Secondary School</h4>
    </div>
</div>

<div class="swiper-slide">
    <div class="testimonial-item">
        <p>
            <i class="bi bi-quote quote-icon-left"></i>
            <span>"The Fecomas School Management System has been a game-changer for our school. The system's features have streamlined our school's operations, and the support team is always available to assist us." - Director, Sosten Gwengwe Foundation</span>
            <i class="bi bi-quote quote-icon-right"></i>
        </p>
        <img src="assets/img/clients/placeholder.jpeg" class="testimonial-img" alt="">
        <h3>Mr. Sosten Gwengwe</h3>
        <h4>The Director, Sosten Gwengwe Foundation</h4>
    </div>
</div>


<div class="swiper-slide">
    <div class="testimonial-item">
        <p>
            <i class="bi bi-quote quote-icon-left"></i>
            <span>"The Fecomas School Management System has exceeded our expectations. The system's features have streamlined our school's operations, and the support team is always available to assist us." - Principal, Izimvale Academy</span>
            <i class="bi bi-quote quote-icon-right"></i>
        </p>
        <img src="assets/img/clients/placeholder.jpeg" class="testimonial-img" alt="">
        <h3>Mr. Issac William</h3>
        <h4>The Principal, Izimvale Academy</h4>
    </div>
</div>


            </div>
            <div class="swiper-pagination"></div>
        </div>

    </div>

</section>        
        <!-- /Testimonials Section -->
<!-- Portfolio Section -->
<!-- Team Section -->
<section id="team" class="team section" style="background-image: url('assets/img/team/team.jpg'); background-size: cover; background-position: center; background-repeat: no-repeat; background-attachment: fixed; background-blend-mode: overlay; background-color: rgba(255, 255, 255, 0.6);">

    <!-- Section Title -->
    <div class="container section-title text-center" data-aos="fade-up">
        <h2 style="color: #001f3d;">Our Team</h2>
        <p style="color: #001f3d;">Our team is dedicated to delivering high-quality software solutions to help your business succeed.</p>
    </div><!-- End Section Title -->

    <div class="container">

        <div class="row gy-4 justify-content-center">

            <div class="col-lg-3 col-md-6 d-flex align-items-stretch" data-aos="fade-up" data-aos-delay="100">
                <div class="team-member text-center">
                    <div class="member-img">
                        <img src="assets/img/Team/Eliot.jpg" class="img-fluid" alt="">
                        <div class="social">
                            <a href=""><i class="bi bi-twitter-x"></i></a>
                            <a href=""><i class="bi bi-facebook"></i></a>
                            <a href=""><i class="bi bi-instagram"></i></a>
                            <a href=""><i class="bi bi-linkedin"></i></a>
                        </div>
                    </div>
                    <div class="member-info">
                        <h4 style="color: #001f3d;">Eliot Kalenga</h4>
                        <span style="color: #001f3d;">Chief Executive Officer</span>
                        <p style="color: #001f3d;">Eliot leads the strategic direction and vision for Fecomas Tech Solutions, ensuring we provide the best software solutions for our clients.</p>
                    </div>
                </div>
            </div><!-- End Team Member -->

            <div class="col-lg-3 col-md-6 d-flex align-items-stretch" data-aos="fade-up" data-aos-delay="200">
                <div class="team-member text-center">
                    <div class="member-img">
                        <img src="assets/img/team/Emmie.jpg" class="img-fluid" alt="">
                        <div class="social">
                            <a href=""><i class="bi bi-twitter-x"></i></a>
                            <a href=""><i class="bi bi-facebook"></i></a>
                            <a href=""><i class="bi bi-instagram"></i></a>
                            <a href=""><i class="bi bi-linkedin"></i></a>
                        </div>
                    </div>
                    <div class="member-info">
                        <h4 style="color: #001f3d;">Emmie Kamtokoma</h4>
                        <span style="color: #001f3d;">Product Manager</span>
                        <p style="color: #001f3d;">Emmie oversees the development and execution of our product roadmap, ensuring that we meet client needs with innovative solutions.</p>
                    </div>
                </div>
            </div><!-- End Team Member -->

            <div class="col-lg-3 col-md-6 d-flex align-items-stretch" data-aos="fade-up" data-aos-delay="300">
                <div class="team-member text-center">
                    <div class="member-img">
                        <img src="assets/img/team/dalo.jpg" class="img-fluid" alt="">
                        <div class="social">
                            <a href=""><i class="bi bi-twitter-x"></i></a>
                            <a href=""><i class="bi bi-facebook"></i></a>
                            <a href=""><i class="bi bi-instagram"></i></a>
                            <a href=""><i class="bi bi-linkedin"></i></a>
                        </div>
                    </div>
                    <div class="member-info">
                        <h4 style="color: #001f3d;">James Dalo</h4>
                        <span style="color: #001f3d;">Chief Technology Officer</span>
                        <p style="color: #001f3d;">James leads the technical team, overseeing the development of our software solutions, ensuring high-quality code and system performance.</p>
                    </div>
                </div>
            </div><!-- End Team Member -->

            <div class="col-lg-3 col-md-6 d-flex align-items-stretch" data-aos="fade-up" data-aos-delay="400">
                <div class="team-member text-center">
                    <div class="member-img">
                        <img src="assets/img/team/febby.jpg" class="img-fluid" alt="">
                        <div class="social">
                            <a href=""><i class="bi bi-twitter-x"></i></a>
                            <a href=""><i class="bi bi-facebook"></i></a>
                            <a href=""><i class="bi bi-instagram"></i></a>
                            <a href=""><i class="bi bi-linkedin"></i></a>
                        </div>
                    </div>
                    <div class="member-info">
                        <h4 style="color: #001f3d;">Febby Nakoma</h4>
                        <span style="color: #001f3d;">Accountant</span>
                        <p style="color: #001f3d;">Febby manages the company's finances, ensuring smooth financial operations, budgeting, and financial planning.</p>
                    </div>
                </div>
            </div><!-- End Team Member -->

        </div>

    </div>

</section>        
        <!-- /Team Section -->
<!-- Contact Section -->
<form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>

    <section id="contact" class="contact section" style="background-image: url('assets/img/bg5.jpg'); background-size: cover; background-position: center; background-repeat: no-repeat; background-attachment: fixed; background-blend-mode: overlay; background-color: rgba(255, 255, 255, 0.9);">
        <div class="container section-title">
            <h2 style="color: #001f3d;">Book a Demo</h2>
            <p style="color: #001f3d;">Schedule a live demo of our software solutions and see how they can benefit your business. Fill out the form below to get started.</p>
        </div>

        <!-- Success Modal -->
        <div class="modal fade" id="successModal" tabindex="-1" role="dialog" aria-labelledby="successModalLabel" aria-hidden="true">
            <div class="modal-dialog" role="document">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title" id="successModalLabel">Success</h5>
                        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                    </div>
                    <div class="modal-body">
                        <asp:Label ID="lblMessage" runat="server" CssClass="text-success">Thank you for submitting your inquiry. Our team will reach out to you soon</asp:Label>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
                    </div>
                </div>
            </div>
        </div>

        <div class="container">
            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                <ContentTemplate>
                    <div class="card p-4 shadow">
                        <div class="row gy-4">
                            <div class="col-lg-12">

                                <div class="mb-3">
                                    <label class="form-label" style="color: #001f3d;">Full Name</label>
                                    <asp:TextBox ID="txtFullName" runat="server" CssClass="form-control" MaxLength="50" style="background-color: rgba(255, 255, 255, 0.8); border: 1px solid #ccc;"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvFullName" runat="server" ControlToValidate="txtFullName"
                                        ErrorMessage="Full Name is required." CssClass="text-danger" Display="Dynamic" />
                                </div>

                                <div class="mb-3">
                                    <label class="form-label" style="color: #001f3d;">Email</label>
                                    <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" MaxLength="50" style="background-color: rgba(255, 255, 255, 0.8); border: 1px solid #ccc;"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvEmail" runat="server" ControlToValidate="txtEmail"
                                        ErrorMessage="Email is required." CssClass="text-danger" Display="Dynamic" />
                                    <asp:RegularExpressionValidator ID="revEmail" runat="server" ControlToValidate="txtEmail"
                                        ErrorMessage="Invalid Email Format." CssClass="text-danger" Display="Dynamic"
                                        ValidationExpression="^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$" />
                                </div>

                                <div class="mb-3">
                                    <label class="form-label" style="color: #001f3d;">Phone</label>
                                    <asp:TextBox ID="txtPhone" runat="server" CssClass="form-control" MaxLength="100" style="background-color: rgba(255, 255, 255, 0.8); border: 1px solid #ccc;"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvPhone" runat="server" ControlToValidate="txtPhone"
                                        ErrorMessage="Phone is required." CssClass="text-danger" Display="Dynamic" />
                                </div>
                                <div class="mb-3">
                                    <label class="form-label" style="color: #001f3d;">Category</label>
                                    <asp:DropDownList ID="ddlCategory" runat="server" CssClass="form-select" style="background-color: rgba(255, 255, 255, 0.8); border: 1px solid #ccc;">
                                        <asp:ListItem Text="-- Select Category --" Value="" />
                                        <asp:ListItem Text="General Inquiry" Value="General" />
                                        <asp:ListItem Text="Support" Value="Support" />
                                        <asp:ListItem Text="Sales" Value="Sales" />
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="rfvCategory" runat="server" ControlToValidate="ddlCategory"
                                        InitialValue="" ErrorMessage="Category is required." CssClass="text-danger" Display="Dynamic" />
                                </div>
                                <div class="mb-3">
                                    <label class="form-label" style="color: #001f3d;">System Interest</label>
                                    <asp:DropDownList ID="ddlSystemInterest" runat="server" CssClass="form-select" style="background-color: rgba(255, 255, 255, 0.8); border: 1px solid #ccc;">
                                        <asp:ListItem Text="-- Select System Interest --" Value="" />
                                        <asp:ListItem Text="Fecomas School Management System" Value="Fecomas School Management System" />
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="rfvSystemInterest" runat="server" ControlToValidate="ddlSystemInterest"
                                        InitialValue="" ErrorMessage="System Interest is required." CssClass="text-danger" Display="Dynamic" />
                                </div>

                                <div class="mb-3">
                                    <label class="form-label" style="color: #001f3d;">Message</label>
<asp:TextBox ID="txtMessage" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="5" style="background-color: rgba(255, 255, 255, 0.8); border: 1px solid #ccc;"></asp:TextBox>                                    <asp:RequiredFieldValidator ID="rfvMessage" runat="server" ControlToValidate="txtMessage"
                                        ErrorMessage="Message is required." CssClass="text-danger" Display="Dynamic" />
                                </div>

                                <div class="mb-3 text-center">
                                    <asp:Button ID="btnSubmit" runat="server" Text="Submit Inquiry" CssClass="btn" style="background-color: #001f3d; color: #fff;" OnClick="btnSubmit_Click" />
                                </div>
                            </div>
                        </div>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </section>
</form>    <style>
        /* Footer Styles */
.footer {
    font-size: 16px; /* Ensure uniform font size */
}

.footer .footer-top h4, 
.footer .footer-about p, 
.footer .footer-links ul li a, 
.footer .info-item h3, 
.footer .info-item p {
    font-size: 16px; /* Set all text elements to the same font size */
}

.footer .sitename {
    font-size: 20px; /* Make the site name slightly larger */
    font-weight: bold;
}

.footer .footer-links ul {
    padding: 0;
    list-style: none;
}

.footer .footer-links ul li {
    margin-bottom: 8px;
}

.footer .info-item {
    display: flex;
    align-items: center;
    gap: 10px;
}

.footer .info-item i {
    font-size: 20px; /* Make icons slightly larger */
    color: var(--primary-color);
}

.footer .social-links a {
    font-size: 20px;
    margin-right: 10px;
    color: #007bff; /* Adjust color if needed */
    transition: color 0.3s;
}

.footer .social-links a:hover {
    color: #0056b3;
}

.copyright {
    font-size: 14px; /* Slightly smaller for copyright text */
}/* Dark Blue Social Links */
.social-links a {
    font-size: 20px;
    color: #00008B; /* Dark Blue */
    margin-right: 10px;
    transition: color 0.3s ease-in-out;
}

.social-links a:hover {
    color: #000066; /* Slightly darker blue on hover */
}


    </style>
    <!-- Footer Section -->
<footer id="footer" class="footer light-background" style="background-image: url('assets/img/bg6.jpg'); background-size: cover; background-position: center;">
    <div class="container footer-top">
        <div class="row gy-4">
            
            <!-- Column 1: About Section -->
            <div class="col-lg-4 col-md-12 footer-about">
                <a href="index.html" class="logo d-flex align-items-center">
                    <span class="sitename">Fecomas School Management System</span>
                </a>
                <p>"Fecomas School Management System: Automate, Simplify, Make Your School Shine. Our comprehensive solution streamlines school operations, empowering educators to focus on delivering exceptional education and fostering student success. Whether you're a preschool, primary school, secondary school, or university, our system is designed to meet the unique needs of your institution, helping you achieve academic excellence."</p>
                <div class="social-links d-flex mt-4 ">
                    <a href="" target="_blank"><i class="bi bi-twitter-x"></i></a>
                    <a href="https://web.facebook.com/FecomasSchoolManagementSystem/" target="_blank"><i class="bi bi-facebook"></i></a>
                    <a href="https://www.instagram.com/fecomastech" target="_blank"><i class="bi bi-instagram"></i></a>
                    <a href="https://www.linkedin.com/company/fecomastech" target="_blank"><i class="bi bi-linkedin"></i></a>
                </div>
            </div>

            <!-- Column 2: Features -->
            <div class="col-lg-4 col-md-6 footer-links">
                <h4>Features</h4>
                <ul>
                    <li><a href="#">Student Registration and Enrollment Management</a></li>
                    <li><a href="#">Examination Management Management</a></li>
                    <li><a href="#">Invocing and Fee Collection Management</a></li>
                    <li><a href="#">Library Management</a></li>
                    <li><a href="#">Attendance Management</a></li>
                    <li><a href="#">Assets Management</a></li>
                    <li><a href="#">Hostels Management</a></li>
                    <li><a href="#">Accounting</a></li>
                    <li><a href="#">Reporting and Analytics</a></li>
                </ul>
            </div>

            <!-- Column 3: Contact Information -->
            <div class="col-lg-4 col-md-6">
                <h4>Our Contact Information</h4>
                <div class="info-item d-flex" data-aos="fade-up" data-aos-delay="200">
                    <i class="bi bi-geo-alt flex-shrink-0"></i>
                    <div>
                        <h3><strong>Physical Address</strong></h3>
                        <p>Area 25/C, Lilongwe, Malawi</p>
                    </div>
                </div>

                <div class="info-item d-flex" data-aos="fade-up" data-aos-delay="300">
                    <i class="bi bi-telephone flex-shrink-0"></i>
                    <div>
                        <h3><strong>Call Us/WhatsApp Us</strong></h3>
                        <p>+265 993 189 671 / +265 886 598 858</p>
                    </div>
                </div>

                <div class="info-item d-flex" data-aos="fade-up" data-aos-delay="400">
                    <i class="bi bi-envelope flex-shrink-0"></i>
                    <div>
                        <h3><strong>Email Us</strong></h3>
                        <p>info@fecomastechsolutions.com</p>
                    </div>
                </div>

                <div class="info-item d-flex" data-aos="fade-up" data-aos-delay="500">
                    <i class="bi bi-globe flex-shrink-0"></i>
                    <div>
                        <h3><strong>Visit our website</strong></h3>
                        <p>www.fecomas.com</p>
                    </div>
                </div>
            </div>

        <div class="container copyright text-center mt-4">
            <p> <span>Copyright</span> <strong class="px-1 sitename">Fecomas School Management System</strong> <span>All Rights Reserved</span></p>
            <div class="credits">
                Developed by <a href="https://fecomas.com/">Fecomas Tech Solutions</a>
            </div>
        </div>
    </div>
</footer>
    <!-- CSS for Font & Footer Styling -->
    <style>
        footer,
        footer * { /* Applies to footer and all its child elements */
            font-family: "Century Gothic", sans-serif !important;
        }
    </style>

    <!-- Scroll Top -->
    <a href="#" id="scroll-top" class="scroll-top d-flex align-items-center justify-content-center"><i class="bi bi-arrow-up-short"></i></a>

    <!-- Preloader -->
<%--    <div id="preloader"></div>--%>

    <!-- Vendor JS Files -->
    <script src="assets/vendor/bootstrap/js/bootstrap.bundle.min.js"></script>
    <script src="assets/vendor/php-email-form/validate.js"></script>
    <script src="assets/vendor/aos/aos.js"></script>
    <script src="assets/vendor/purecounter/purecounter_vanilla.js"></script>
    <script src="assets/vendor/swiper/swiper-bundle.min.js"></script>
    <script src="assets/vendor/glightbox/js/glightbox.min.js"></script>
    <script src="assets/vendor/imagesloaded/imagesloaded.pkgd.min.js"></script>
    <script src="assets/vendor/isotope-layout/isotope.pkgd.min.js"></script>

    <!-- Main JS File -->
    <script src="assets/js/main.js">


    </script>

    <script>

        document.addEventListener("DOMContentLoaded", function () {
            // Get all iframes on the page
            const iframes = document.querySelectorAll("iframe");

            // Listen for postMessage events from YouTube players
            window.addEventListener("message", function (event) {
                // Ignore non-YouTube messages
                if (typeof event.data !== "string" || event.data.indexOf("infoDelivery") === -1) return;

                // Loop through all iframes
                iframes.forEach((iframe) => {
                    // Skip if this is the one currently playing
                    if (event.source === iframe.contentWindow) return;

                    // Send pause command to every other YouTube video
                    iframe.contentWindow.postMessage('{"event":"command","func":"pauseVideo","args":""}', '*');
                });
            });

            // Enable JS API for all YouTube iframes
            iframes.forEach((iframe) => {
                if (iframe.src.includes("youtube.com/embed")) {
                    if (iframe.src.indexOf("enablejsapi=1") === -1) {
                        const joinChar = iframe.src.includes("?") ? "&" : "?";
                        iframe.src += joinChar + "enablejsapi=1";
                    }
                }
            });
        });
        const clientCount = document.getElementById('client-count');
        const count = 36;

        function animateCount() {
            let currentCount = 0;
            const interval = setInterval(() => {
                if (currentCount >= count) {
                    clearInterval(interval);
                } else {
                    currentCount++;
                    clientCount.textContent = currentCount;
                }
            }, 100); // adjust the speed of the count
        }

        // Check if the section is visible
        const observer = new IntersectionObserver((entries) => {
            if (entries[0].isIntersecting) {
                animateCount();
                observer.unobserve(clientCount);
            }
        }, { threshold: 0.5 });

        observer.observe(clientCount);
    </script>
</body>

</html>