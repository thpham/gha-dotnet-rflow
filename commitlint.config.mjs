export default {
  extends: ['@commitlint/config-conventional'],
  rules: {
    'type-enum': [
      2,
      'always',
      [
        'feat',     // New feature
        'fix',      // Bug fix
        'security', // Security fix or improvement
        'docs',     // Documentation
        'style',    // Formatting, missing semicolons, etc.
        'refactor', // Code refactoring
        'perf',     // Performance improvements
        'test',     // Adding or updating tests
        'build',    // Build system or external dependencies
        'ci',       // CI/CD configuration
        'chore',    // Maintenance tasks
        'revert'    // Revert a previous commit
      ]
    ],
    'scope-enum': [
      1,  // Warning level (not error)
      'always',
      [
        'app',
        'api',
        'tests',
        'installer',
        'deps',
        'ci',
        'release'
      ]
    ],
    'subject-case': [
      2,
      'never',
      ['start-case', 'pascal-case', 'upper-case']
    ],
    'subject-max-length': [2, 'always', 72],
    'body-max-line-length': [2, 'always', 100],
    'header-max-length': [2, 'always', 100]
  }
};
