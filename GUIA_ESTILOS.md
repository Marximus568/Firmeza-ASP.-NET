# 🎨 Guía Rápida de Estilos - AdminDashboard

## 🚀 Inicio Rápido

### Para ver las mejoras visuales:

1. **Ejecutar el proyecto:**
```bash
cd /home/Coder/RiderProjects/Firmeza-ASP.-NET/AdminDashboard.Web
dotnet run
```

2. **Navegar a:** `https://localhost:5001`

---

## 📚 Clases CSS Útiles

### Animaciones
```html
<!-- Animación de entrada con fade -->
<div class="animate-fade-in-up">Contenido</div>

<!-- Animación lateral -->
<div class="animate-slide-in-left">Contenido</div>

<!-- Con delays -->
<div class="animate-fade-in-up" style="animation-delay: 0.1s;">Card 1</div>
<div class="animate-fade-in-up" style="animation-delay: 0.2s;">Card 2</div>
```

### Efectos Visuales
```html
<!-- Texto con gradiente -->
<h1 class="text-gradient">Título Premium</h1>

<!-- Efecto glow en sombra -->
<button class="btn btn-primary shadow-glow">Button</button>

<!-- Efecto glassmorphism -->
<div class="glass-effect">Contenido</div>
```

### Cards Mejoradas
```html
<!-- Card con sombra grande -->
<div class="card shadow-lg">
  <div class="card-header">
    <h5 class="mb-0">
      <i class="fas fa-icon me-2"></i>Título
    </h5>
  </div>
  <div class="card-body">
    Contenido
  </div>
</div>

<!-- Card de estadística con gradiente -->
<div class="card text-white bg-primary">
  <div class="card-body text-center p-4">
    <div class="mb-3">
      <i class="fas fa-chart-line" style="font-size: 3rem;"></i>
    </div>
    <h5 class="card-title text-uppercase">Título</h5>
    <h2 class="mb-0" style="font-size: 3rem; font-weight: 800;">123</h2>
  </div>
</div>
```

### Botones con Gradiente
```html
<!-- Botón primario con gradiente -->
<button class="btn btn-primary">Acción</button>

<!-- Botón con icono -->
<button class="btn btn-success">
  <i class="fas fa-plus me-2"></i>Crear
</button>

<!-- Botón grande con glow -->
<button class="btn btn-primary btn-lg shadow-glow">Premium</button>
```

### Formularios con Iconos
```html
<div class="mb-3">
  <label class="form-label">
    <i class="fas fa-search me-2"></i>Buscar
  </label>
  <input type="text" class="form-control" placeholder="Ingrese texto...">
</div>
```

### Tables Estilizadas
```html
<div class="table-responsive">
  <table class="table table-hover">
    <thead>
      <tr>
        <th>Columna 1</th>
        <th>Columna 2</th>
      </tr>
    </thead>
    <tbody>
      <tr>
        <td>Dato 1</td>
        <td>Dato 2</td>
      </tr>
    </tbody>
  </table>
</div>
```

### Badges con Gradiente
```html
<span class="badge bg-primary">Primary</span>
<span class="badge bg-success">Success</span>
<span class="badge bg-warning">Warning</span>
<span class="badge bg-danger">Danger</span>
<span class="badge bg-info">Info</span>
```

---

## 🎨 Variables CSS Personalizables

### En `site.css` puedes modificar:

```css
:root {
  /* Colores principales */
  --color-primary: #6366f1;
  --color-secondary: #8b5cf6;
  --color-accent: #ec4899;
  
  /* Espaciado */
  --spacing-sm: 0.5rem;
  --spacing-md: 1rem;
  --spacing-lg: 1.5rem;
  
  /* Border Radius */
  --radius-lg: 0.75rem;
  --radius-xl: 1rem;
  
  /* Sombras */
  --shadow-lg: 0 10px 15px -3px rgba(0, 0, 0, 0.1);
  --shadow-glow: 0 0 20px rgba(99, 102, 241, 0.4);
  
  /* Transiciones */
  --transition-base: 300ms ease-in-out;
}
```

---

## 🎯 Iconos Font Awesome

### Iconos comunes usados:

```html
<!-- Navegación -->
<i class="fas fa-home"></i>
<i class="fas fa-users"></i>
<i class="fas fa-box"></i>
<i class="fas fa-chart-line"></i>

<!-- Acciones -->
<i class="fas fa-plus"></i>
<i class="fas fa-edit"></i>
<i class="fas fa-trash"></i>
<i class="fas fa-search"></i>

<!-- Estados -->
<i class="fas fa-check"></i>
<i class="fas fa-times"></i>
<i class="fas fa-exclamation-triangle"></i>
<i class="fas fa-info-circle"></i>
```

---

## 📐 Estructura de Layout Recomendada

```html
<div class="container-fluid animate-fade-in-up">
  <!-- Header -->
  <div class="row mb-4">
    <div class="col-12">
      <h1 class="text-gradient" style="font-size: 2.5rem; font-weight: 800;">
        <i class="fas fa-icon me-3"></i>Título
      </h1>
      <p class="text-muted" style="font-size: 1.1rem;">
        <i class="fas fa-info-circle me-2"></i>Descripción
      </p>
    </div>
  </div>
  
  <!-- Stats Cards -->
  <div class="row mb-4 g-4">
    <div class="col-md-3 animate-fade-in-up" style="animation-delay: 0.1s;">
      <!-- Card aquí -->
    </div>
  </div>
  
  <!-- Filters -->
  <div class="card mb-4 shadow-lg">
    <!-- Filtros aquí -->
  </div>
  
  <!-- Data Table -->
  <div class="card shadow-lg">
    <!-- Tabla aquí -->
  </div>
</div>
```

---

## 🔧 Tips de Desarrollo

### 1. Hover Effects
```css
.elemento:hover {
  transform: translateY(-4px);
  box-shadow: var(--shadow-xl);
  transition: all var(--transition-base);
}
```

### 2. Responsive Spacing
```html
<!-- Usa las clases de Bootstrap + nuestro spacing -->
<div class="p-4 mb-4 g-4">
  <!-- Contenido -->
</div>
```

### 3. Gradientes Personalizados
```css
background: linear-gradient(135deg, #color1 0%, #color2 100%);
```

---

## 📱 Breakpoints Responsive

```css
/* Mobile First */
@media (min-width: 768px) {
  /* Tablet */
}

@media (min-width: 992px) {
  /* Desktop */
}

@media (min-width: 1200px) {
  /* Large Desktop */
}
```

---

## 🎬 Animaciones Timing

```css
/* Delays recomendados para secuencias */
style="animation-delay: 0.1s;"  /* Primera card */
style="animation-delay: 0.2s;"  /* Segunda card */
style="animation-delay: 0.3s;"  /* Tercera card */
style="animation-delay: 0.4s;"  /* Cuarta card */
```

---

## 🚨 Alerts Estilizadas

```html
<div class="alert alert-success" role="alert">
  <i class="fas fa-check-circle me-2"></i>
  Operación exitosa
</div>

<div class="alert alert-danger" role="alert">
  <i class="fas fa-exclamation-circle me-2"></i>
  Error en la operación
</div>

<div class="alert alert-warning" role="alert">
  <i class="fas fa-exclamation-triangle me-2"></i>
  Advertencia importante
</div>

<div class="alert alert-info" role="alert">
  <i class="fas fa-info-circle me-2"></i>
  Información útil
</div>
```

---

## 🎨 Colores Exactos del Sistema

| Nombre | Hex | Uso |
|--------|-----|-----|
| Primary | `#6366f1` | Botones principales, links |
| Secondary | `#8b5cf6` | Elementos secundarios |
| Success | `#10b981` | Confirmaciones, éxitos |
| Warning | `#f59e0b` | Advertencias |
| Error | `#ef4444` | Errores, eliminaciones |
| Info | `#3b82f6` | Información |

---

**💡 Tip Final**: Todos estos estilos están diseñados para funcionar juntos. Mezcla y combina según necesites!
