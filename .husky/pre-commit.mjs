import { execSync } from 'node:child_process';

function run(command) {
  execSync(command, { stdio: 'inherit' });
}

try {
  run('npm run test:all');
} catch {
  process.exit(1);
}
