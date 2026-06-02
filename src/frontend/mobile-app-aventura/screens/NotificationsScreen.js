import React, { useState } from 'react';

import {
  View,
  StyleSheet,
  ScrollView,
  SafeAreaView,
} from 'react-native';

import BottomTabBar from '../components/BottomTabBar';
import InfoCard from '../components/InfoCard';
import NotificationToggleItem from '../components/NotificationToggleItem';
import SettingsSection from '../components/SettingsSection';
import ScreenHeader from '../components/ScreenHeader';
import SystemStatus from '../components/SystemStatus';

import useNotificationsViewModel from '../viewmodels/useNotificationsViewModel';

import { colors } from '../constants/colors';
import { spacing } from '../constants/spacing';

export default function NotificationsScreen({
  navigation,
}) {
  const [systemStatus, setSystemStatus] = useState(null);

  const {
    sections,
    handleToggleOption,
  } = useNotificationsViewModel();

  function handleToggleWithStatus(sectionId, optionId) {
    const result = handleToggleOption(sectionId, optionId);

    setSystemStatus({
      type: result?.enabled ? 'success' : 'info',
      message: result?.enabled
        ? `${result.title} ativado.`
        : `${result?.title || 'Notificação'} desativado.`,
    });
  }

  return (
    <View style={styles.page}>
      <SafeAreaView style={styles.contentSafeArea}>
        <ScrollView
          showsVerticalScrollIndicator={false}
          contentContainerStyle={styles.scrollContent}
        >
          <ScreenHeader
            title="Notificações"
            onBackPress={() =>
              navigation.navigate('MainTabs', { screen: 'Profile' })
            }
          />

          <InfoCard
            title="Gerencie suas notificações"
            description="Escolha quais alertas deseja receber sobre novos espetáculos e eventos."
          />

          {sections.map((section) => (
            <SettingsSection
              key={section.id}
              title={section.title}
            >
              {section.options.map((option, index) => (
                <NotificationToggleItem
                  key={option.id}
                  title={option.title}
                  subtitle={option.subtitle}
                  enabled={option.enabled}
                  onToggle={() =>
                    handleToggleWithStatus(
                      section.id,
                      option.id
                    )
                  }
                  showDivider={
                    index !== section.options.length - 1
                  }
                />
              ))}
            </SettingsSection>
          ))}
        </ScrollView>
      </SafeAreaView>

      <BottomTabBar
        activeTab="profile"
        onTabPress={(tab) => {
          if (tab === 'explore') {
            navigation.navigate('MainTabs', {
              screen: 'Home',
            });

            return;
          }

          if (tab === 'profile') {
            navigation.navigate('MainTabs', { screen: 'Profile' });
          }
        }}
      />

      <SystemStatus
        message={systemStatus?.message}
        type={systemStatus?.type}
        onDismiss={() => setSystemStatus(null)}
      />
    </View>
  );
}

const styles = StyleSheet.create({
  page: {
    flex: 1,
    backgroundColor: colors.whiteSoft,
  },

  contentSafeArea: {
    flex: 1,
    backgroundColor: colors.whiteSoft,
  },

  scrollContent: {
    paddingHorizontal: spacing.md,
    paddingTop: spacing.md,
    paddingBottom: 140,
    gap: spacing.lg,
  },
});
