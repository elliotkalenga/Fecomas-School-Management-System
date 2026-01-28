<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UserLogin.aspx.cs" Inherits="SMSWEBAPP.Views.Admin.UserLogin" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Fecomas School Management System</title>
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <link href="https://maxcdn.bootstrapcdn.com/bootstrap/4.5.2/css/bootstrap.min.css" rel="stylesheet" />
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/5.15.3/css/all.min.css" />

    <style>
        * {
    font-family: "Century Gothic", "Segoe UI", Arial, sans-serif !important;
}
.fa,
.fas,
.fa-solid,
.far,
.fa-regular {
    font-family: "Font Awesome 5 Free" !important;
    font-weight: 900;
}

.fab,
.fa-brands {
    font-family: "Font Awesome 5 Brands" !important;
    font-weight: normal !important;
}

        body, html {
            height: 100%;
            margin: 0;
            font-family: 'Segoe UI', sans-serif;
            background: linear-gradient(to right, rgba(0, 31, 63, 0.8), rgba(0, 51, 102, 0.8)), 
                        url('ftslogo.png') no-repeat center center fixed;
            background-size: cover;
            display: flex;
            justify-content: center;
            align-items: center;
        }

        .login-wrapper {
            max-width: 420px;
            width: 100%;
            padding: 30px;
            background: rgba(255, 255, 255, 0.15);
            backdrop-filter: blur(10px);
            border-radius: 20px;
            box-shadow: 0 10px 25px rgba(0, 0, 0, 0.3);
            text-align: center;
        }

        .login-wrapper .logo {
            font-size: 80px;
            color: white;
        }

        .login-wrapper h4 {
            color: #ffa500;
            font-weight: bold;
            margin-top: 15px;
        }

        .slogan {
            color: #f0f0f0;
            font-style: italic;
            margin-bottom: 25px;
        }

        .form-group .input-group-text {
            background-color: #001f3f;
            color: white;
        }

        .form-control {
            border-radius: 0.5rem !important;
        }

        .btn-dark-blue {
            background-color: #001f3f !important;
            border: none;
            color: white;
            font-weight: bold;
            transition: 0.3s;
        }

        .btn-dark-blue:hover {
            background-color: #004080 !important;
        }

        .center-links {
            margin-top: 20px;
            color: #fff;
            font-size: 0.95em;
        }

        .center-links a {
            color: #fff !important;
            text-decoration: none;
        }

        .center-links a:hover {
            color: #ffeb3b !important;
        }

        .modal-content iframe {
            border-radius: 10px;
        }

        .main-icon {
            font-size: 120px;
            color: white;
            background-color: rgba(255, 255, 255, 0.2);
            border-radius: 50%;
            padding: 25px;
            margin-bottom: 20px;
        }

        @media (max-width: 480px) {
            .login-wrapper {
                margin: 10px;
                padding: 20px;
            }
        }
        /* ===== NETWORK ANIMATION BACKGROUND ===== */
#networkCanvas {
    position: fixed;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    z-index: 0;
}

/* Keep login card above animation */
.login-wrapper {
    position: relative;
    z-index: 2;
}

    </style>
</head>
<body>
    <canvas id="networkCanvas"></canvas>

    <form id="form1" runat="server">
        <div class="login-wrapper">
            <i class="fas fa-graduation-cap main-icon"></i>
            <h4>FECOMAS SCHOOL MANAGEMENT SYSTEM</h4>
            <div class="slogan">Work Smarter, Efficiency Assured</div>

            <asp:Label ID="lblError" runat="server" ForeColor="#ff4c4c" Text="" Visible="False" Font-Size="Medium"></asp:Label>

            <div class="form-group">
                <div class="input-group">
                    <div class="input-group-prepend">
                        <span class="input-group-text"><i class="fas fa-user"></i></span>
                    </div>
                    <asp:TextBox runat="server" type="text" class="form-control" ID="TxtUsername" placeholder="Enter username" required autocomplete="off" />
                </div>
            </div>

            <div class="form-group">
                <div class="input-group">
                    <div class="input-group-prepend">
                        <span class="input-group-text"><i class="fas fa-lock"></i></span>
                    </div>
                    <asp:TextBox runat="server" ID="TxtPassword" type="password" class="form-control" placeholder="Enter password" required autocomplete="off"/>
                </div>
            </div>

            <asp:Button runat="server" ID="BtnLogin" type="submit" class="btn btn-dark-blue btn-block" Text="Login" OnClick="BtnLogin_Click" />

            <div class="center-links mt-3">
                <a href="#" data-toggle="modal" data-target="#recoverModal"><i class="fas fa-key"></i> Recover Login Details for Students</a>
            </div>

            <div class="center-links mt-4">
                <a href="https://fecomas.com">&copy; Fecomas Tech Solutions <span id="current-year"></span></a>
                <br />
                <a href="https://wa.me/265993189671" target="_blank">
                    <i class="fab fa-whatsapp" style="font-size: 20px; color: #25D366;"></i> WhatsApp Us for Quick Feedback
                </a>
                <br />
                <a href="https://fecomas.com/Guide/termsandconditions.pdf" target="_blank">Terms and Conditions</a>
                <br />
                <a href="https://fecomas.com/Guide/PRIVACYPOLICY.pdf" target="_blank">Privacy Policy</a>
            </div>
        </div>
    </form>

    <!-- Recover Login Modal -->
    <div class="modal fade" id="recoverModal" tabindex="-1" role="dialog" aria-labelledby="recoverModalLabel" aria-hidden="true">
        <div class="modal-dialog modal-custom" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" id="recoverModalLabel"><i class="fas fa-key"></i> Recover Login Details</h5>
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                        <span aria-hidden="true">&times;</span>
                    </button>
                </div>
                <div class="modal-body">
                    <iframe src="../Students/RecoverLogin.aspx" width="100%" height="500px" style="border:none;"></iframe>
                </div>
            </div>
        </div>
    </div>

    <script>
        document.getElementById("current-year").innerText = new Date().getFullYear();
const canvas = document.getElementById("networkCanvas");
const ctx = canvas.getContext("2d");

let width, height;
let particles = [];

/* ===== TUNING VALUES ===== */
const particleCount = 110;        // MORE dots = complete mesh
const maxDistance = 180;          // Longer connections
const speedMultiplier = 1.4;      // Faster movement
const dotColor = "rgba(0, 28, 64, 0.95)";   // VERY dark blue
const lineColorBase = "0, 28, 64";          // RGB only

function resizeCanvas() {
    width = canvas.width = window.innerWidth;
    height = canvas.height = window.innerHeight;
}
window.addEventListener("resize", resizeCanvas);
resizeCanvas();

class Particle {
    constructor() {
        this.x = Math.random() * width;
        this.y = Math.random() * height;
        this.vx = (Math.random() - 0.5) * speedMultiplier;
        this.vy = (Math.random() - 0.5) * speedMultiplier;
        this.radius = 2.2;
    }

    move() {
        this.x += this.vx;
        this.y += this.vy;

        if (this.x <= 0 || this.x >= width) this.vx *= -1;
        if (this.y <= 0 || this.y >= height) this.vy *= -1;
    }

    draw() {
        ctx.beginPath();
        ctx.arc(this.x, this.y, this.radius, 0, Math.PI * 2);
        ctx.fillStyle = dotColor;
        ctx.fill();
    }
}

function connectParticles() {
    for (let i = 0; i < particles.length; i++) {
        for (let j = i + 1; j < particles.length; j++) {
            const dx = particles[i].x - particles[j].x;
            const dy = particles[i].y - particles[j].y;
            const distance = Math.sqrt(dx * dx + dy * dy);

            if (distance < maxDistance) {
                const opacity = 1 - (distance / maxDistance);
                ctx.beginPath();
                ctx.strokeStyle = `rgba(${lineColorBase}, ${opacity * 0.85})`;
                ctx.lineWidth = 1;
                ctx.moveTo(particles[i].x, particles[i].y);
                ctx.lineTo(particles[j].x, particles[j].y);
                ctx.stroke();
            }
        }
    }
}

function animate() {
    ctx.clearRect(0, 0, width, height);

    particles.forEach(p => {
        p.move();
        p.draw();
    });

    connectParticles();
    requestAnimationFrame(animate);
}

/* INIT */
particles = [];
for (let i = 0; i < particleCount; i++) {
    particles.push(new Particle());
}
animate();
    </script>

    <script src="https://code.jquery.com/jquery-3.5.1.slim.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/@popperjs/core@2.5.3/dist/umd/popper.min.js"></script>
    <script src="https://maxcdn.bootstrapcdn.com/bootstrap/4.5.2/js/bootstrap.min.js"></script>
</body>
</html>