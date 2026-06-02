import React, { useState } from 'react';

import {
  View,
  Text,
  StyleSheet,
  ScrollView,
  SafeAreaView,
} from 'react-native';

import BottomTabBar from '../components/BottomTabBar';
import ContactCard from '../components/ContactCard';
import SettingsSection from '../components/SettingsSection';
import SocialLinkCard from '../components/SocialLinkCard';
import ScreenHeader from '../components/ScreenHeader';
import SystemStatus from '../components/SystemStatus';

import useInstitutionalViewModel from '../viewmodels/useInstitutionalViewModel';

import { colors } from '../constants/colors';
import { typography } from '../constants/typography';
import { radius, spacing } from '../constants/spacing';

export default function InstitutionalScreen({
  navigation,
}) {
  const [systemStatus, setSystemStatus] = useState(null);

  const {
    aboutText,
    socialLinks,
    contactInfo,

    handleSocialPress,
  } = useInstitutionalViewModel();

  function handleSocialPressWithStatus(link) {
    handleSocialPress(link);
    setSystemStatus({
      type: 'info',
      message: `Abrindo ${link.platform}.`,
    });
  }

  function handleContactPressWithStatus(info) {
    setSystemStatus({
      type: 'info',
      message: `Abrindo ${info.label}.`,
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
            title="Informações"
            onBackPress={() =>
              navigation.navigate('MainTabs', { screen: 'Profile' })
            }
          />

          <View style={styles.aboutCard}>
            <Text style={styles.sectionTitle}>
              Sobre a Aventura
            </Text>

            <Text style={styles.about}>
              {aboutText}
            </Text>
          </View>

          <SettingsSection title="Siga-nos">
            {socialLinks.map((link, index) => (
              <SocialLinkCard
                key={link.id}
                icon={link.icon}
                platform={link.platform}
                username={link.username}
                onPress={() => handleSocialPressWithStatus(link)}
                showDivider={
                  index !== socialLinks.length - 1
                }
              />
            ))}
          </SettingsSection>

          {contactInfo.map((info) => (
            <SettingsSection
              key={info.id}
              title={info.label}
            >
              <ContactCard
                icon={info.icon}
                value={info.value}
                link={
                  info.id !== 'location'
                }
                onPress={() => handleContactPressWithStatus(info)}
              />
            </SettingsSection>
          ))}

          <View style={styles.footer}>
            <Text style={styles.footerText}>
              Aventura Teatros © 2008-2025
            </Text>

            <Text style={styles.footerText}>
              Todos os direitos reservados.
            </Text>

            <Text style={styles.footerText}>
              Desenvolvido com carinho para a cultura brasileira
            </Text>
          </View>
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

  aboutCard: {
    backgroundColor: colors.white,
    borderWidth: 1,
    borderColor: colors.grayLight,
    borderRadius: radius.md,

    padding: spacing.lg,
    gap: spacing.sm,

    shadowColor: colors.black,
    shadowOffset: {
      width: 0,
      height: 4,
    },
    shadowOpacity: 0.04,
    shadowRadius: 10,
    elevation: 2,
  },

  sectionTitle: {
    fontSize: typography.size.md,
    lineHeight: typography.lineHeight.md,
    fontWeight: typography.fontWeight.semiBold,
    color: colors.grayDark,
  },

  about: {
    fontSize: typography.size.sm,
    lineHeight: typography.lineHeight.md,
    color: colors.grayText,
  },

  footer: {
    marginTop: spacing.xl,
    alignItems: 'center',
    gap: spacing.xs,
  },

  footerText: {
    textAlign: 'center',
    fontSize: typography.size.sm,
    lineHeight: typography.lineHeight.sm,
    color: colors.grayText,
  },
});
