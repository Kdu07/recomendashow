import { useEffect, useState } from 'react';

import {
  getNotificationData,
  updateNotificationPreference,
} from '../repositories/notificationRepository';

export default function useNotificationsViewModel() {
  const [sections, setSections] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    let mounted = true;

    async function load() {
      try {
        const data = await getNotificationData();
        if (mounted) setSections(data.sections || []);
      } catch (error) {
        console.warn('Erro ao carregar notificacoes:', error);
      } finally {
        if (mounted) setLoading(false);
      }
    }

    load();

    return () => {
      mounted = false;
    };
  }, []);

  async function handleToggleOption(sectionId, optionId) {
    const currentSection = sections.find((section) => section.id === sectionId);
    const currentOption = currentSection?.options.find((option) => option.id === optionId);
    const nextEnabled = !currentOption?.enabled;

    setSections((previous) =>
      previous.map((section) => ({
        ...section,
        options: section.options.map((option) =>
          option.id === optionId
            ? { ...option, enabled: nextEnabled }
            : option
        ),
      }))
    );

    const saved = await updateNotificationPreference(nextEnabled);

    if (!saved) {
      setSections((previous) =>
        previous.map((section) => ({
          ...section,
          options: section.options.map((option) =>
            option.id === optionId
              ? { ...option, enabled: currentOption?.enabled ?? false }
              : option
          ),
        }))
      );
    }

    return {
      title: currentOption?.title ?? 'Notificacao',
      enabled: saved ? nextEnabled : currentOption?.enabled ?? false,
      saved,
    };
  }

  return {
    sections,
    loading,
    handleToggleOption,
  };
}
