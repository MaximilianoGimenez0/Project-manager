export const pages = {
  home: {
    hero: {
      title1: 'Moderniza el Flujo de Aprobación',
      title2: 'de tus Proyectos',
      subtitle: '{appName} es la solución definitiva para proponer, gestionar y aprobar proyectos sin problemas. Elimina los cuellos de botella y aporta transparencia a las decisiones organizativas.',
      btnCreate: 'Crear una Propuesta',
      btnExplore: 'Explorar Proyectos',
      preview: {
        badge: 'En Revisión',
        code: 'PRJ-2026-04',
        title: 'Modernización de Infraestructura',
        desc: 'Migración de servicios centrales a arquitectura de alta disponibilidad con canalizaciones automatizadas.',
        budgetLabel: 'Presupuesto',
        budgetValue: '$48,500',
        durationLabel: 'Duración',
        durationValue: '6 meses',
        areaLabel: 'Área',
        areaValue: 'Ingeniería',
        step1: 'Auditoría Técnica',
        step2: 'Evaluación Presupuestaria',
        step3: 'Aprobación Ejecutiva'
      }
    },
    overview: {
      title: '¿Por qué {appName}?',
      subtitle: 'La gestión de proyectos tradicional se basa en correos electrónicos fragmentados y documentos perdidos. Creamos un centro unificado donde cada propuesta de proyecto recibe la visibilidad, el escrutinio y las decisiones oportunas que merece.'
    },
    features: {
      management: {
        title: 'Gestión de Propuestas',
        desc: 'Envía propuestas de proyectos estructuradas con toda la información necesaria centralizada en un solo lugar.'
      },
      workflows: {
        title: 'Flujos de Aprobación',
        desc: 'Procesos de toma de decisiones optimizados para que los administradores revisen, aprueben o rechacen proyectos.'
      },
      search: {
        title: 'Búsqueda Avanzada',
        desc: 'Encuentra proyectos específicos al instante utilizando potentes filtros por estado, creador o roles asignados.'
      },
      roles: {
        title: 'Decisiones Basadas en Roles',
        desc: 'Asegura que las partes interesadas adecuadas tomen decisiones en la etapa correcta del ciclo de vida del proyecto.'
      }
    },
    howItWorks: {
      title: 'Cómo Funciona',
      step1: { title: 'Crear Propuesta', desc: 'Redacta y envía tu idea de proyecto.' },
      step2: { title: 'Enviar Proyecto', desc: 'Envía la propuesta para su revisión oficial.' },
      step3: { title: 'Proceso de Revisión', desc: 'Los evaluadores analizan la viabilidad y el impacto.' },
      step4: { title: 'Decisión de Aprobación', desc: 'Veredicto final: Aprobado, Rechazado o Reelaborar.' },
      step5: { title: 'Seguimiento', desc: 'Supervisa el progreso y las decisiones históricas.' }
    },
    tech: {
      title: 'Construido para Escalar',
      desc: 'Impulsado por una arquitectura moderna usando React, Vite y TypeScript. Diseñado siguiendo estrictos principios de arquitectura limpia, asegurando alto rendimiento, mantenibilidad y confiabilidad a nivel empresarial.'
    }
  },
  about: {
    header: {
      title: 'Acerca de {appName}',
      subtitle: 'Un sistema moderno, transparente y eficiente de aprobación de proyectos diseñado para escala y claridad.'
    },
    challenge: {
      title: 'El Desafío',
      desc: 'Las organizaciones a menudo tienen problemas con la gestión de propuestas de proyectos. Las aprobaciones se pierden en hilos de correo electrónico, los comentarios están desarticulados y el estado actual de una iniciativa rara vez está claro para todas las partes interesadas. Esta falta de transparencia provoca cuellos de botella, retrasos en la innovación y frustración.'
    },
    solution: {
      title: 'La Solución',
      desc: '{appName} fue creado para centralizar y estandarizar el ciclo de vida de las propuestas de proyectos. Al pasar de una comunicación no estructurada a un flujo de trabajo definido, los equipos pueden concentrarse en evaluar ideas en lugar de rastrearlas.',
      hub: 'Centro Centralizado:',
      hubDesc: 'Todos los datos del proyecto, discusiones y decisiones en un solo lugar.',
      roles: 'Acceso Basado en Roles:',
      rolesDesc: 'Clara separación entre creadores, revisores y aprobadores.',
      audit: 'Pista de Auditoría:',
      auditDesc: 'Un historial transparente de todas las decisiones tomadas sobre un proyecto.'
    },
    tech: {
      title: 'Pila Tecnológica',
      desc: 'Esta plataforma está construida con una arquitectura frontend moderna enfocada en el rendimiento, la mantenibilidad y la experiencia del desarrollador.',
      react: 'Librería UI',
      vite: 'Herramienta de Construcción',
      ts: 'Seguridad de Tipos',
      css: 'Estilización'
    },
    roadmap: {
      title: 'Hoja de Ruta Futura',
      desc: 'Estamos evolucionando constantemente {appName}. Las próximas características incluyen:',
      collab: 'Colaboración en tiempo real y comentarios sobre propuestas.',
      analytics: 'Paneles de análisis y reportes avanzados.',
      integrations: 'Integración con herramientas de gestión de tareas de terceros (por ejemplo, Jira, Trello).',
      custom: 'Flujos de aprobación personalizables por departamento.'
    }
  },
  createProject: {
    header: {
      title: 'Borrador de Propuesta',
      subtitle: 'Complete los detalles para presentar la propuesta al comité de evaluación.'
    },
    guidance: {
      title: 'Directrices',
      titleLabel: 'Título:',
      titleDesc: 'Hágalo conciso y descriptivo. Debe ser fácilmente identificable.',
      descLabel: 'Descripción:',
      descDesc: 'Indique claramente el objetivo, alcance y resultados esperados.',
      typeLabel: 'Tipo:',
      typeDesc: 'Categorice su proyecto correctamente para que los expertos adecuados puedan evaluarlo.',
      areaLabel: 'Área:',
      areaDesc: 'Seleccione el departamento principal que será dueño de los resultados.',
      durationLabel: 'Duración:',
      durationDesc: 'Proporcione una estimación realista en días para ayudar a asignar recursos.',
      amountLabel: 'Monto:',
      amountDesc: 'Indique el monto aproximado para el proyecto.'
    },
    messages: {
      success: 'Propuesta de proyecto enviada exitosamente.',
      error: 'Ha ocurrido un error'
    }
  },
  pending: {
    header: {
      title: 'Panel de Aprobaciones',
      subtitle: 'Revise y tome decisiones sobre propuestas de proyectos pendientes.'
    },
    loading: 'Cargando aprobaciones pendientes...',
    errorTitle: 'Error al cargar',
    emptyTitle: '¡Todo al día!',
    emptyDesc: 'No hay proyectos pendientes que requieran su aprobación en este momento.',
    resultsCount: '{count} propuesta(s) espera(n) su decisión',
    refresh: 'Actualizar',
    retry: 'Reintentar',
    searchPlaceholder: 'Buscar por título, solicitante o área...',
    clearSearch: 'Limpiar búsqueda',
    noMatchesTitle: 'Sin coincidencias',
    noMatchesDesc: 'No encontramos proyectos que coincidan con su búsqueda.',
    actionRequiredBadge: 'Tu turno de decidir',
    viewDetails: 'Ver detalle',
    stats: {
      total: 'Total Pendientes',
      actionNeeded: 'Requieren tu Decisión',
      totalAmount: 'Presupuesto en Revisión'
    }
  },
  search: {
    header: {
      title: 'Explorar Proyectos',
      subtitle: 'Filtre y descubra propuestas de proyectos en toda la organización.'
    },
    filters: 'Filtros',
    loading: 'Buscando proyectos...',
    errorTitle: 'Error al cargar proyectos',
    resultsCount: 'Se encontraron {count} proyecto(s)',
    emptyTitle: 'No se encontraron proyectos',
    emptyDesc: 'No pudimos encontrar ningún proyecto que coincida con sus filtros actuales. Intente ajustar sus criterios de búsqueda.',
    clearFilters: 'Limpiar filtros',
    createNew: 'Proponer un proyecto'
  },
  login: {
    title: 'Iniciar Sesión',
    emailLabel: 'Correo Electrónico',
    emailPlaceholder: 'ejemplo@unaj.com',
    passwordLabel: 'Contraseña',
    passwordPlaceholder: '••••••••',
    submitButton: 'Iniciar Sesión',
    submittingButton: 'Iniciando sesión...',
    errorTitle: 'Error de autenticación',
    successMessage: 'Bienvenido de nuevo, {name}',
    demoAccountsTitle: 'Usuarios de prueba disponibles',
    demoAccountsSubtitle: 'Haga clic en un usuario para autocompletar credenciales',
    role: 'Rol',
    showPassword: 'Mostrar contraseña',
    hidePassword: 'Ocultar contraseña',
    errors: {
      emailRequired: 'El correo electrónico es requerido',
      emailInvalid: 'Ingrese un formato de correo válido',
      passwordRequired: 'La contraseña es requerida',
    }
  },
  projectDetail: {
    backToSearch: 'Volver a la búsqueda',
    loading: 'Cargando detalles del proyecto...',
    errorTitle: 'Error al cargar el proyecto',
    notFound: 'Proyecto no encontrado',
    notFoundDesc: 'La propuesta solicitada no existe o no se tienen permisos para verla.',
    code: 'Código',
    details: 'Detalles del Proyecto',
    description: 'Memoria Descriptiva',
    applicant: 'Datos del Solicitante',
    applicantName: 'Nombre',
    applicantEmail: 'Correo',
    applicantRole: 'Rol',
    budget: 'Presupuesto',
    duration: 'Duración',
    area: 'Área',
    type: 'Tipo',
    months: 'meses',
    notAvailable: 'No disponible',
    nextApproverTitle: 'Próximo Rol de Aprobador Requerido',
    nextApproverStep: 'Paso {current} de {total}',
    nextApproverSubtitle: 'Esta propuesta está esperando la evaluación de un:',
    nextApproverSubtitleRole: 'Esta propuesta está esperando la evaluación de: {role}',
    workflowApproved: 'Circuito completado con éxito: La propuesta ha sido Aprobada.',
    workflowRejected: 'Circuito finalizado: La propuesta ha sido Rechazada.',
    workflowFinished: 'Flujo de aprobación finalizado',
    userCanDecide: 'Tienes el rol requerido ({role}) para evaluar este paso.',
    userCannotDecide: 'Esperando intervención del rol correspondiente.',
    btnDecide: 'Evaluar Paso Ahora',
    approvalWorkflow: 'Circuito y Etapas de Aprobación',
    stepNumber: 'Paso {order}',
    stepOrderAndRole: 'Paso {order} — {role}',
    approverRole: 'Rol Evaluador',
    decidedBy: 'Decidido por',
    decisionDate: 'Fecha de decisión',
    observations: 'Observaciones',
    pendingEvaluation: 'Pendiente de evaluación',
    decisionSuccess: 'Decisión registrada exitosamente',
    decisionError: 'Error al registrar la decisión',
    nextApproverAriaLabel: 'Estado del próximo aprobador',
    metricsAriaLabel: 'Métricas principales del proyecto',
    applicantAriaLabel: 'Información del solicitante',
  },
  notFound: {
    code: '404',
    title: 'Página no encontrada',
    description: 'La página que está buscando no existe o ha sido movida.',
    backHome: 'Volver al Inicio'
  }
} as const;

