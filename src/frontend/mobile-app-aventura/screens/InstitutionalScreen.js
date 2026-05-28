import React from 'react';

import {
  View,
  Text,
  StyleSheet,
  ScrollView,
} from 'react-native';

import BottomTabBar from '../components/BottomTabBar';
import ContactCard from '../components/ContactCard';
import SettingsSection from '../components/SettingsSection';
import SocialLinkCard from '../components/SocialLinkCard';
import ScreenHeader from '../components/ScreenHeader';

import useInstitutionalViewModel from '../viewmodels/useInstitutionalViewModel';

import { colors } from '../constants/colors';
import { typography } from '../constants/typography';
import { spacing } from '../constants/spacing';

export default function InstitutionalScreen({
  navigation,
}) {
  const {
    aboutText,
    socialLinks,
    contactInfo,

    handleSocialPress,
  } = useInstitutionalViewModel();

  return (
    <View style={styles.page}>
      <ScrollView
        showsVerticalScrollIndicator={false}
        contentContainerStyle={styles.scrollContent}
      >
        <ScreenHeader
          title="INFORMAÇÕES INSTITUCIONAIS"
          onBackPress={() =>
            navigation.navigate('Profile')
          }
        />

        <View style={styles.section}>
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
              onPress={() =>
                handleSocialPress(link)
              }
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
            Desenvolvido com ♥ para a cultura brasileira
          </Text>
        </View>
      </ScrollView>

      <BottomTabBar
        activeTab="profile"
        onTabPress={(tab) => {
          if (tab === 'explore') {
            navigation.navigate('Home');
            return;
          }

          if (tab === 'profile') {
            navigation.navigate('Profile');
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

  section: {
    gap: spacing.md,
  },

  sectionTitle: {
    fontSize: typography.size.lg,
    fontWeight: typography.fontWeight.semiBold,
    color: colors.grayDark,
  },

  about: {
    fontSize: typography.size.lg,
    color: colors.grayText,
    lineHeight: 36,
  },

  footer: {
    marginTop: spacing.xxl,
    alignItems: 'center',
    gap: spacing.sm,
  },

  footerText: {
    textAlign: 'center',
    fontSize: typography.size.sm,
    color: colors.grayText,
    lineHeight: 24,
  },
});