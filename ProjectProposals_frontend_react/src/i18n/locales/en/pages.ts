export const pages = {
  home: {
    hero: {
      title1: 'Modernize Your Project',
      title2: 'Approval Workflow',
      subtitle: '{appName} is the definitive solution to propose, manage, and approve projects seamlessly. Eliminate bottlenecks and bring transparency to your organizational decisions.',
      btnCreate: 'Create a Proposal',
      btnExplore: 'Explore Projects',
      preview: {
        badge: 'In Review',
        code: 'PRJ-2026-04',
        title: 'Infrastructure Modernization',
        desc: 'Migrate core services to high-availability architecture with automated pipelines.',
        budgetLabel: 'Budget',
        budgetValue: '$48,500',
        durationLabel: 'Duration',
        durationValue: '6 months',
        areaLabel: 'Area',
        areaValue: 'Engineering',
        step1: 'Technical Audit',
        step2: 'Budget Evaluation',
        step3: 'Executive Sign-off'
      }
    },
    overview: {
      title: 'Why {appName}?',
      subtitle: 'Traditional project management relies on fragmented emails and lost documents. We created a unified hub where every project proposal receives the visibility, scrutiny, and timely decisions it deserves.'
    },
    features: {
      management: {
        title: 'Proposal Management',
        desc: 'Submit structured project proposals with all necessary information centralized in one place.'
      },
      workflows: {
        title: 'Approval Workflows',
        desc: 'Streamlined decision-making processes for administrators to review, approve, or reject projects.'
      },
      search: {
        title: 'Advanced Search',
        desc: 'Find specific projects instantly using powerful filtering by status, creator, or assigned roles.'
      },
      roles: {
        title: 'Role-Based Decisions',
        desc: 'Ensure that the right stakeholders are making decisions at the right stage of the project lifecycle.'
      }
    },
    howItWorks: {
      title: 'How It Works',
      step1: { title: 'Create Proposal', desc: 'Draft and submit your project idea.' },
      step2: { title: 'Submit Project', desc: 'Send the proposal for official review.' },
      step3: { title: 'Review Process', desc: 'Evaluators analyze feasibility and impact.' },
      step4: { title: 'Approval Decision', desc: 'Final verdict: Approved, Rejected, or Rework.' },
      step5: { title: 'Tracking', desc: 'Monitor progress and historical decisions.' }
    },
    tech: {
      title: 'Built for Scale',
      desc: 'Powered by a modern architecture using React, Vite, and TypeScript. Designed following strict clean architecture principles, ensuring high performance, maintainability, and enterprise-grade reliability.'
    }
  },
  about: {
    header: {
      title: 'About {appName}',
      subtitle: 'A modern, transparent, and efficient project approval system designed for scale and clarity.'
    },
    challenge: {
      title: 'The Challenge',
      desc: 'Organizations often struggle with project proposal management. Approvals get lost in email threads, feedback is disjointed, and the current status of an initiative is rarely clear to all stakeholders. This lack of transparency leads to bottlenecks, delayed innovation, and frustration.'
    },
    solution: {
      title: 'The Solution',
      desc: '{appName} was created to centralize and standardize the lifecycle of project proposals. By moving from unstructured communication to a defined workflow, teams can focus on evaluating ideas rather than tracking them down.',
      hub: 'Centralized Hub:',
      hubDesc: 'All project data, discussions, and decisions in one place.',
      roles: 'Role-Based Access:',
      rolesDesc: 'Clear separation between creators, reviewers, and approvers.',
      audit: 'Audit Trail:',
      auditDesc: 'A transparent history of all decisions made on a project.'
    },
    tech: {
      title: 'Technology Stack',
      desc: 'This platform is built with a modern frontend architecture focused on performance, maintainability, and developer experience.',
      react: 'UI Library',
      vite: 'Build Tool',
      ts: 'Type Safety',
      css: 'Styling'
    },
    roadmap: {
      title: 'Future Roadmap',
      desc: 'We are constantly evolving {appName}. Upcoming features include:',
      collab: 'Real-time collaboration and commenting on proposals.',
      analytics: 'Advanced analytics and reporting dashboards.',
      integrations: 'Integration with third-party task management tools (e.g., Jira, Trello).',
      custom: 'Customizable approval workflows per department.'
    }
  },
  createProject: {
    header: {
      title: 'Draft Proposal',
      subtitle: 'Complete the details to submit your proposal to the evaluation committee.'
    },
    guidance: {
      title: 'Guidelines',
      titleLabel: 'Title:',
      titleDesc: 'Make it concise and descriptive. It should be easily identifiable.',
      descLabel: 'Description:',
      descDesc: 'Clearly state the objective, scope, and expected outcomes.',
      typeLabel: 'Type:',
      typeDesc: 'Categorize your project correctly so the right experts can evaluate it.',
      areaLabel: 'Area:',
      areaDesc: 'Select the primary department that will own the results.',
      durationLabel: 'Duration:',
      durationDesc: 'Provide a realistic estimate in days to help allocate resources.',
      amountLabel: 'Amount:',
      amountDesc: 'Indicate the approximate amount for the project.'
    },
    messages: {
      success: 'Project proposal submitted successfully.',
      error: 'An error occurred'
    }
  },
  pending: {
    header: {
      title: 'Approvals Dashboard',
      subtitle: 'Review and make decisions on pending project proposals.'
    },
    loading: 'Loading pending approvals...',
    errorTitle: 'Failed to load',
    emptyTitle: 'All caught up!',
    emptyDesc: 'There are no pending projects requiring your approval at this moment.',
    resultsCount: '{count} proposal(s) await your decision',
    refresh: 'Refresh',
    retry: 'Retry',
    searchPlaceholder: 'Search by title, applicant or area...',
    clearSearch: 'Clear search',
    noMatchesTitle: 'No matches found',
    noMatchesDesc: 'We could not find any projects matching your search.',
    actionRequiredBadge: 'Your turn to decide',
    viewDetails: 'View details',
    stats: {
      total: 'Total Pending',
      actionNeeded: 'Action Required',
      totalAmount: 'Budget Under Review'
    }
  },
  search: {
    header: {
      title: 'Explore Projects',
      subtitle: 'Filter and discover project proposals across the organization.'
    },
    filters: 'Filters',
    loading: 'Searching projects...',
    errorTitle: 'Error loading projects',
    resultsCount: 'Found {count} project(s)',
    emptyTitle: 'No projects found',
    emptyDesc: 'We couldn\'t find any projects matching your current filters. Try adjusting your search criteria.',
    clearFilters: 'Clear filters',
    createNew: 'Propose a project'
  },
  login: {
    title: 'Sign In',
    emailLabel: 'Email Address',
    emailPlaceholder: 'example@unaj.com',
    passwordLabel: 'Password',
    passwordPlaceholder: '••••••••',
    submitButton: 'Sign In',
    submittingButton: 'Signing in...',
    errorTitle: 'Authentication Error',
    successMessage: 'Welcome back, {name}',
    demoAccountsTitle: 'Available Test Users',
    demoAccountsSubtitle: 'Click on a user to autofill credentials',
    role: 'Role',
    showPassword: 'Show password',
    hidePassword: 'Hide password',
    errors: {
      emailRequired: 'Email is required',
      emailInvalid: 'Please enter a valid email address',
      passwordRequired: 'Password is required',
    }
  },
  projectDetail: {
    backToSearch: 'Back to Search',
    loading: 'Loading project details...',
    errorTitle: 'Error loading project',
    notFound: 'Project Not Found',
    notFoundDesc: 'The requested project proposal does not exist or you lack permission to view it.',
    code: 'Code',
    details: 'Project Details',
    description: 'Project Overview & Scope',
    applicant: 'Applicant Details',
    applicantName: 'Name',
    applicantEmail: 'Email',
    applicantRole: 'Role',
    budget: 'Budget',
    duration: 'Duration',
    area: 'Area',
    type: 'Type',
    months: 'months',
    notAvailable: 'N/A',
    nextApproverTitle: 'Next Required Approver Role',
    nextApproverStep: 'Step {current} of {total}',
    nextApproverSubtitle: 'This proposal is currently waiting for evaluation by a:',
    nextApproverSubtitleRole: 'This proposal is currently waiting for evaluation by: {role}',
    workflowApproved: 'Workflow completed successfully: The proposal is Approved.',
    workflowRejected: 'Workflow concluded: The proposal was Rejected.',
    workflowFinished: 'Approval workflow completed',
    userCanDecide: 'You have the required role ({role}) to evaluate this step.',
    userCannotDecide: 'Awaiting action from the designated reviewer role.',
    btnDecide: 'Evaluate Step Now',
    approvalWorkflow: 'Approval Stages & History',
    stepNumber: 'Step {order}',
    stepOrderAndRole: 'Step {order} — {role}',
    approverRole: 'Reviewer Role',
    decidedBy: 'Decided by',
    decisionDate: 'Decision Date',
    observations: 'Observations',
    pendingEvaluation: 'Pending evaluation',
    decisionSuccess: 'Decision recorded successfully',
    decisionError: 'Failed to record decision',
    nextApproverAriaLabel: 'Next approver status',
    metricsAriaLabel: 'Key project metrics',
    applicantAriaLabel: 'Applicant information',
  },
  notFound: {
    code: '404',
    title: 'Page Not Found',
    description: 'The page you are looking for does not exist or has been moved.',
    backHome: 'Back to Dashboard'
  }
} as const;

