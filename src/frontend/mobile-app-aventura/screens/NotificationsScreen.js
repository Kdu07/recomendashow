import React from 'react';

import {
  View,
  Text,
  StyleSheet,
  ScrollView,
} from 'react-native';

import BottomTabBar from '../components/BottomTabBar';
import InfoCard from '../components/InfoCard';
import NotificationToggleItem from '../components/NotificationToggleItem';
import SettingsSection from '../components/SettingsSection';
import ScreenHeader from '../components/ScreenHeader';

import useNotificationsViewModel from '../viewmodels/useNotificationsViewModel';

import { colors } from '../constants/colors';
import { typography } from '../constants/typography';
import { spacing } from '../constants/spacing';

export default function NotificationsScreen({
  navigation,
}) {
  const {
    sections,
    handleToggleOption,
  } = useNotificationsViewModel();

  return (
    <View style={styles.page}>
      <ScrollView
        showsVerticalScrollIndicator={false}
        contentContainerStyle={styles.scrollContent}
      >
        <ScreenHeader
          title="NOTIFICAÇÕES"
          onBackPress={() =>
            navigation.navigate('MainTabs', { screen: 'Profile', })
          }
        />

        <InfoCard
          title="Gerencie suas notificações"
          description="Escolha quais alertas você deseja receber e mantenha-se informado sobre novos espetáculos e eventos"
        />

        {sections.map((section) => (
          <SettingsSection
            key={section.id}
            title={section.title}
          >
            {section.options.map(
              (option, index) => (
                <NotificationToggleItem
                  key={option.id}
                  title={option.title}
                  subtitle={option.subtitle}
                  enabled={option.enabled}
                  onToggle={() =>
                    handleToggleOption(
                      section.id,
                      option.id
                    )
                  }
                  showDivider={
                    index !==
                    section.options.length - 1
                  }
                />
              )
            )}
          </SettingsSection>
        ))}
      </ScrollView>

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
            navigation.navigate("MainTabs", { screen: "Profile", });
          }
        }}
      />
    </View>
  );
}

const styles = StyleSheet.create({
  page: {
    flex: 1,
    backgroundColor: '#FAFAFA',
  },

  scrollContent: {
    padding: spacing.lg,
    paddingBottom: 140,
    gap: spacing.xl,
  },

  header: {
    fontSize: typography.size.md,
    fontWeight: typography.fontWeight.bold,
    color: colors.grayDark,
  },
});