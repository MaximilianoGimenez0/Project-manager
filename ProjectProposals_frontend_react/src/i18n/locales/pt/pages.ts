export const pages = {
  home: {
    hero: {
      title1: 'Modernize o Fluxo de Aprovação',
      title2: 'dos Seus Projetos',
      subtitle: '{appName} é a solução definitiva para propor, gerenciar e aprovar projetos sem problemas. Elimine gargalos e traga transparência às decisões organizacionais.',
      btnCreate: 'Criar uma Proposta',
      btnExplore: 'Explorar Projetos',
      preview: {
        badge: 'Em Revisão',
        code: 'PRJ-2026-04',
        title: 'Modernização de Infraestrutura',
        desc: 'Migrar serviços centrais para arquitetura de alta disponibilidade com pipelines automatizados.',
        budgetLabel: 'Orçamento',
        budgetValue: '$48,500',
        durationLabel: 'Duração',
        durationValue: '6 meses',
        areaLabel: 'Área',
        areaValue: 'Engenharia',
        step1: 'Auditoria Técnica',
        step2: 'Avaliação Orçamentária',
        step3: 'Aprovação Executiva'
      }
    },
    overview: {
      title: 'Por que {appName}?',
      subtitle: 'O gerenciamento de projetos tradicional depende de e-mails fragmentados e documentos perdidos. Criamos um hub unificado onde cada proposta de projeto recebe a visibilidade, o escrutínio e as decisões oportunas que merece.'
    },
    features: {
      management: {
        title: 'Gerenciamento de Propostas',
        desc: 'Envie propostas de projetos estruturadas com todas as informações necessárias centralizadas em um só lugar.'
      },
      workflows: {
        title: 'Fluxos de Aprovação',
        desc: 'Processos de tomada de decisão simplificados para administradores revisarem, aprovarem ou rejeitarem projetos.'
      },
      search: {
        title: 'Busca Avançada',
        desc: 'Encontre projetos específicos instantaneamente usando filtros poderosos por status, criador ou funções atribuídas.'
      },
      roles: {
        title: 'Decisões Baseadas em Funções',
        desc: 'Garante que as partes interessadas certas tomem decisões no estágio certo do ciclo de vida do projeto.'
      }
    },
    howItWorks: {
      title: 'Como Funciona',
      step1: { title: 'Criar Proposta', desc: 'Esboce e envie sua ideia de projeto.' },
      step2: { title: 'Enviar Projeto', desc: 'Envie a proposta para revisão oficial.' },
      step3: { title: 'Processo de Revisão', desc: 'Avaliadores analisam viabilidade e impacto.' },
      step4: { title: 'Decisão de Aprovação', desc: 'Veredicto final: Aprovado, Rejeitado ou Retrabalho.' },
      step5: { title: 'Acompanhamento', desc: 'Monitore o progresso e decisões históricas.' }
    },
    tech: {
      title: 'Construído para Escalar',
      desc: 'Alimentado por uma arquitetura moderna usando React, Vite e TypeScript. Projetado seguindo rígidos princípios de arquitetura limpa, garantindo alto desempenho, facilidade de manutenção e confiabilidade de nível empresarial.'
    }
  },
  about: {
    header: {
      title: 'Sobre {appName}',
      subtitle: 'Um sistema moderno, transparente e eficiente de aprovação de projetos projetado para escala e clareza.'
    },
    challenge: {
      title: 'O Desafio',
      desc: 'As organizações muitas vezes têm dificuldades com o gerenciamento de propostas de projetos. Aprovações se perdem em tópicos de e-mail, o feedback é desarticulado e o status atual de uma iniciativa raramente é claro para todas as partes interessadas. Essa falta de transparência leva a gargalos, inovação atrasada e frustração.'
    },
    solution: {
      title: 'A Solução',
      desc: '{appName} foi criado para centralizar e padronizar o ciclo de vida das propostas de projeto. Ao mudar de uma comunicação não estruturada para um fluxo de trabalho definido, as equipes podem se concentrar em avaliar ideias em vez de rastreá-las.',
      hub: 'Hub Centralizado:',
      hubDesc: 'Todos os dados do projeto, discussões e decisões em um só lugar.',
      roles: 'Acesso Baseado em Função:',
      rolesDesc: 'Separação clara entre criadores, revisores e aprovadores.',
      audit: 'Trilha de Auditoria:',
      auditDesc: 'Um histórico transparente de todas as decisões tomadas sobre um projeto.'
    },
    tech: {
      title: 'Pilha de Tecnologia',
      desc: 'Esta plataforma é construída com uma arquitetura frontend moderna focada em desempenho, facilidade de manutenção e experiência do desenvolvedor.',
      react: 'Biblioteca UI',
      vite: 'Ferramenta de Construção',
      ts: 'Segurança de Tipos',
      css: 'Estilização'
    },
    roadmap: {
      title: 'Roteiro Futuro',
      desc: 'Estamos em constante evolução no {appName}. Os próximos recursos incluem:',
      collab: 'Colaboração em tempo real e comentários em propostas.',
      analytics: 'Painéis avançados de análise e relatórios.',
      integrations: 'Integração com ferramentas de gerenciamento de tarefas de terceiros (ex: Jira, Trello).',
      custom: 'Fluxos de trabalho de aprovação personalizáveis por departamento.'
    }
  },
  createProject: {
    header: {
      title: 'Rascunho da Proposta',
      subtitle: 'Preencha os detalhes para apresentar a proposta ao comitê de avaliação.'
    },
    guidance: {
      title: 'Diretrizes',
      titleLabel: 'Título:',
      titleDesc: 'Seja conciso e descritivo. Deve ser facilmente identificável.',
      descLabel: 'Descrição:',
      descDesc: 'Indique claramente o objetivo, escopo e resultados esperados.',
      typeLabel: 'Tipo:',
      typeDesc: 'Categorize seu projeto corretamente para que os especialistas certos possam avaliá-lo.',
      areaLabel: 'Área:',
      areaDesc: 'Selecione o departamento principal que será dono dos resultados.',
      durationLabel: 'Duração:',
      durationDesc: 'Forneça uma estimativa realista em dias para ajudar a alocar recursos.',
      amountLabel: 'Valor:',
      amountDesc: 'Indique o valor aproximado para o projeto.'
    },
    messages: {
      success: 'Proposta de projeto enviada com sucesso.',
      error: 'Ocorreu um erro'
    }
  },
  pending: {
    header: {
      title: 'Painel de Aprovações',
      subtitle: 'Revise e tome decisões sobre propostas de projetos pendentes.'
    },
    loading: 'Carregando aprovações pendentes...',
    errorTitle: 'Falha ao carregar',
    emptyTitle: 'Tudo em dia!',
    emptyDesc: 'Não há projetos pendentes que exijam sua aprovação no momento.',
    resultsCount: '{count} proposta(s) aguardam sua decisão',
    refresh: 'Atualizar',
    retry: 'Tentar novamente',
    searchPlaceholder: 'Pesquisar por título, solicitante ou área...',
    clearSearch: 'Limpar pesquisa',
    noMatchesTitle: 'Sem resultados',
    noMatchesDesc: 'Não encontramos projetos que correspondam à sua pesquisa.',
    actionRequiredBadge: 'Sua vez de decidir',
    viewDetails: 'Ver detalhes',
    stats: {
      total: 'Total Pendentes',
      actionNeeded: 'Requerem sua Decisão',
      totalAmount: 'Orçamento em Revisão'
    }
  },
  search: {
    header: {
      title: 'Explorar Projetos',
      subtitle: 'Filtre e descubra propostas de projetos em toda a organização.'
    },
    filters: 'Filtros',
    loading: 'Pesquisando projetos...',
    errorTitle: 'Erro ao carregar projetos',
    resultsCount: 'Encontrado(s) {count} projeto(s)',
    emptyTitle: 'Nenhum projeto encontrado',
    emptyDesc: 'Não conseguimos encontrar nenhum projeto correspondente aos seus filtros atuais. Tente ajustar seus critérios de pesquisa.',
    clearFilters: 'Limpar filtros',
    createNew: 'Propor um projeto'
  },
  login: {
    title: 'Entrar no Sistema',
    emailLabel: 'E-mail',
    emailPlaceholder: 'exemplo@unaj.com',
    passwordLabel: 'Senha',
    passwordPlaceholder: '••••••••',
    submitButton: 'Entrar',
    submittingButton: 'Entrando...',
    errorTitle: 'Erro de autenticação',
    successMessage: 'Bem-vindo de volta, {name}',
    demoAccountsTitle: 'Usuários de teste disponíveis',
    demoAccountsSubtitle: 'Clique em um usuário para preencher as credenciais',
    role: 'Função',
    showPassword: 'Ver senha',
    hidePassword: 'Ocultar senha',
    errors: {
      emailRequired: 'O e-mail é obrigatório',
      emailInvalid: 'Insira um formato de e-mail válido',
      passwordRequired: 'A senha é obrigatória',
    }
  },
  projectDetail: {
    backToSearch: 'Voltar para a busca',
    loading: 'Carregando detalhes do projeto...',
    errorTitle: 'Erro ao carregar o projeto',
    notFound: 'Projeto não encontrado',
    notFoundDesc: 'A proposta solicitada não existe ou você não tem permissão para visualizá-la.',
    code: 'Código',
    details: 'Detalhes do Projeto',
    description: 'Memorial Descritivo',
    applicant: 'Dados do Solicitante',
    applicantName: 'Nome',
    applicantEmail: 'E-mail',
    applicantRole: 'Função',
    budget: 'Orçamento',
    duration: 'Duração',
    area: 'Área',
    type: 'Tipo',
    months: 'meses',
    notAvailable: 'Não disponível',
    nextApproverTitle: 'Próxima Função Aprovadora Necessária',
    nextApproverStep: 'Etapa {current} de {total}',
    nextApproverSubtitle: 'Esta proposta está aguardando avaliação de um(a):',
    nextApproverSubtitleRole: 'Esta proposta está aguardando avaliação de: {role}',
    workflowApproved: 'Circuito concluído com sucesso: A proposta foi Aprovada.',
    workflowRejected: 'Circuito encerrado: A proposta foi Rejeitada.',
    workflowFinished: 'Fluxo de aprovação encerrado',
    userCanDecide: 'Você tem a função necessária ({role}) para avaliar esta etapa.',
    userCannotDecide: 'Aguardando intervenção da função correspondente.',
    btnDecide: 'Avaliar Etapa Agora',
    approvalWorkflow: 'Circuito e Etapas de Aprovação',
    stepNumber: 'Etapa {order}',
    stepOrderAndRole: 'Etapa {order} — {role}',
    approverRole: 'Função Avaliadora',
    decidedBy: 'Decidido por',
    decisionDate: 'Data da decisão',
    observations: 'Observações',
    pendingEvaluation: 'Pendente de avaliação',
    decisionSuccess: 'Decisão registrada com sucesso',
    decisionError: 'Erro ao registrar a decisão',
    nextApproverAriaLabel: 'Status do próximo aprovador',
    metricsAriaLabel: 'Métricas principais do projeto',
    applicantAriaLabel: 'Informações do solicitante',
  },
  notFound: {
    code: '404',
    title: 'Página não encontrada',
    description: 'A página que você está procurando não existe ou foi movida.',
    backHome: 'Voltar ao Painel'
  }
} as const;

