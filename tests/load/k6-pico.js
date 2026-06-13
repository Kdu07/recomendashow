// Teste de carga — RNF06 (Projeto.md, seções 10 e 17.4)
//
// Fluxo simulado: home (catálogo + gêneros + teatros) → carrosséis de
// recomendação → wishlist, replicando a navegação típica do app.
//
// Cenários (selecionados via -e SCENARIO=<nome>):
//   pico   (positivo) — rampa 0→50→100 VUs em 5 min, sustenta 100 VUs por 15 min.
//                       Metas: média ≤ 3s, p95 ≤ 5s, erro < 1%.
//   stress (negativo) — 150 VUs por 10 min, observando degradação elegante.
//   spike  (negativo) — 0→100 VUs em 30s, observando recuperação em ≤ 60s.
//   rampa1000         — etapa adicional 100→500→1.000 VUs (critério da seção 10).
//                       SÓ RODAR COM AUTORIZAÇÃO DO TIME: risco de derrubar a
//                       VM única da OCI. Monitorar CPU/memória durante o teste.
//   smoke             — 1 VU / 30s, para validar o script e medir RNF01 (p95).
//
// Uso:
//   k6 run -e SCENARIO=smoke tests/load/k6-pico.js
//   k6 run -e SCENARIO=pico -e BASE_URL=http://163.176.204.208:5253 \
//          -e LOGIN_EMAIL=<email> -e LOGIN_SENHA=<senha> tests/load/k6-pico.js
//
// A API exige JWT em todas as rotas exceto /api/auth/*; o setup() faz login
// uma vez e compartilha o token entre os VUs.

import http from 'k6/http';
import { check, group, sleep } from 'k6';
import { Trend } from 'k6/metrics';

const BASE_URL = __ENV.BASE_URL || 'http://163.176.204.208:5253';
const LOGIN_EMAIL = __ENV.LOGIN_EMAIL || 'teste.carga@aventura.com';
const LOGIN_SENHA = __ENV.LOGIN_SENHA || 'SenhaDeTeste123!';
const SCENARIO = __ENV.SCENARIO || 'smoke';

const homeTrend = new Trend('home_duration', true);
const recsTrend = new Trend('recs_duration', true);
const wishlistTrend = new Trend('wishlist_duration', true);

const scenarios = {
  smoke: {
    executor: 'constant-vus',
    vus: 1,
    duration: '30s',
  },
  pico: {
    executor: 'ramping-vus',
    startVUs: 0,
    stages: [
      { duration: '2m30s', target: 50 },
      { duration: '2m30s', target: 100 },
      { duration: '15m', target: 100 },
      { duration: '1m', target: 0 },
    ],
  },
  stress: {
    executor: 'ramping-vus',
    startVUs: 0,
    stages: [
      { duration: '1m', target: 150 },
      { duration: '10m', target: 150 },
      { duration: '1m', target: 0 },
    ],
  },
  spike: {
    executor: 'ramping-vus',
    startVUs: 0,
    stages: [
      { duration: '30s', target: 100 },
      { duration: '2m', target: 100 },
      { duration: '30s', target: 0 },
    ],
  },
  rampa1000: {
    executor: 'ramping-vus',
    startVUs: 0,
    stages: [
      { duration: '2m', target: 100 },
      { duration: '3m', target: 500 },
      { duration: '5m', target: 1000 },
      { duration: '2m', target: 0 },
    ],
  },
};

export const options = {
  scenarios: { [SCENARIO]: scenarios[SCENARIO] },
  thresholds: {
    // Metas do cenário positivo de 17.4 (média ≤ 3s, p95 ≤ 5s, erro < 1%)
    http_req_duration: ['avg<3000', 'p(95)<5000'],
    http_req_failed: ['rate<0.01'],
  },
};

export function setup() {
  const res = http.post(
    `${BASE_URL}/api/auth/login`,
    JSON.stringify({ email: LOGIN_EMAIL, senha: LOGIN_SENHA }),
    { headers: { 'Content-Type': 'application/json' } },
  );
  check(res, { 'login ok': (r) => r.status === 200 });
  if (res.status !== 200) {
    throw new Error(`Login falhou (${res.status}): crie o usuário de teste ou passe LOGIN_EMAIL/LOGIN_SENHA.`);
  }
  const body = res.json();
  return { token: body.token, usuarioId: body.usuarioId };
}

export default function (data) {
  const params = {
    headers: {
      Authorization: `Bearer ${data.token}`,
      'Content-Type': 'application/json',
    },
  };

  group('home', () => {
    const responses = http.batch([
      ['GET', `${BASE_URL}/api/espetaculo`, null, params],
      ['GET', `${BASE_URL}/api/genero`, null, params],
      ['GET', `${BASE_URL}/api/teatro`, null, params],
    ]);
    responses.forEach((r) => {
      homeTrend.add(r.timings.duration);
      check(r, { 'home 200': (res) => res.status === 200 });
    });
  });

  sleep(Math.random() * 2 + 1);

  group('carrosseis-recomendacao', () => {
    const r = http.get(`${BASE_URL}/api/recomendacao/${data.usuarioId}`, params);
    recsTrend.add(r.timings.duration);
    check(r, { 'recomendacao 200': (res) => res.status === 200 });
  });

  sleep(Math.random() * 2 + 1);

  group('wishlist', () => {
    const r = http.get(`${BASE_URL}/api/wishlist/${data.usuarioId}`, params);
    wishlistTrend.add(r.timings.duration);
    check(r, { 'wishlist 200': (res) => res.status === 200 });
  });

  sleep(Math.random() * 3 + 1);
}
