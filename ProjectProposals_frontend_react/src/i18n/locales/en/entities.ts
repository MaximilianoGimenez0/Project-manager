export const entities = {
  roles: {
    1: 'Area Leader',
    2: 'Manager',
    3: 'Director',
    4: 'Technical Committee',
  },
  statuses: {
    1: 'Pending',
    2: 'Approved',
    3: 'Rejected',
    4: 'Observed',
  },
  areas: {
    1: 'Finance',
    2: 'Technology',
    3: 'Human Resources',
    4: 'Operations',
  },
  types: {
    1: 'Process Improvement',
    2: 'Innovation and Development',
    3: 'Infrastructure',
    4: 'Internal Training',
  },
} as const;

