import { getPreferences, updatePreferences } from './preferencesRepository';

function buildSections(enabled) {
  return [
    {
      id: 'notifications',
      title: 'NOTIFICACOES',
      options: [
        {
          id: 'all',
          title: 'Receber notificacoes',
          subtitle: 'Alertas sobre espetaculos, recomendacoes e novidades',
          enabled,
        },
      ],
    },
  ];
}

export async function getNotificationData() {
  const preferences = await getPreferences();
  const enabled = preferences?.ReceberNotificacoes ?? preferences?.receberNotificacoes ?? true;

  return {
    sections: buildSections(enabled),
  };
}

export async function updateNotificationPreference(enabled) {
  const preferences = await getPreferences();
  const updated = await updatePreferences({
    Generos: preferences?.Generos ?? preferences?.generos ?? [],
    Tags: preferences?.Tags ?? preferences?.tags ?? [],
    Cep: preferences?.Cep ?? preferences?.cep ?? null,
    Cidade: preferences?.Cidade ?? preferences?.cidade ?? null,
    FaixaPrecoMin: preferences?.FaixaPrecoMin ?? preferences?.faixaPrecoMin ?? null,
    FaixaPrecoMax: preferences?.FaixaPrecoMax ?? preferences?.faixaPrecoMax ?? null,
    ReceberNotificacoes: enabled,
  });

  return Boolean(updated);
}
