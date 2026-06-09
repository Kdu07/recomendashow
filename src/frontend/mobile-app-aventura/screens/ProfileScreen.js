import React, { useCallback } from 'react';

import {
  Alert,
  View,
  Text,
  StyleSheet,
  ScrollView,
  TouchableOpacity,
  SafeAreaView,
} from 'react-native';
import { Ionicons } from '@expo/vector-icons';
import { useFocusEffect } from '@react-navigation/native';
import { ActivityIndicator } from 'react-native';

import useProfileViewModel from '../viewmodels/useProfileViewModel';

import { colors } from '../constants/colors';
import { typography } from '../constants/typography';
import { radius, spacing } from '../constants/spacing';

function MenuItem({
  icon,
  title,
  subtitle,
  onPress,
}) {
  return (
    <TouchableOpacity
      style={styles.menuItem}
      onPress={onPress}
      activeOpacity={0.8}
    >
      <View style={styles.menuLeft}>
        <View style={styles.iconContainer}>
          <Ionicons name={icon} size={22} color={colors.purplePrimary} />
        </View>

        <View style={styles.menuCopy}>
          <Text style={styles.menuTitle}>
            {title}
          </Text>

          <Text style={styles.menuSubtitle}>
            {subtitle}
          </Text>
        </View>
      </View>

      <View style={styles.chevronContainer}>
        <Ionicons name="chevron-forward" size={20} color={colors.grayText} />
      </View>
    </TouchableOpacity>
  );
}

export default function ProfileScreen({ navigation }) {
  const {
    user,
    menuItems,
    refreshProfile,
    isLoading,
    deletingAccount,

    handleMenuPress,
    handleEditProfile,
    handleLogout,
    handleDeleteAccount,
  } = useProfileViewModel(navigation);

  function confirmDeleteAccount() {
    Alert.alert(
      'Excluir conta',
      'Seus dados serão anonimizados em até 30 dias, conforme a LGPD. Essa ação não pode ser desfeita.',
      [
        { text: 'Cancelar', style: 'cancel' },
        {
          text: 'Excluir conta',
          style: 'destructive',
          onPress: async () => {
            try {
              await handleDeleteAccount();
              navigation.replace('Auth');
            } catch (error) {
              Alert.alert('Erro', error.message);
            }
          },
        },
      ],
    );
  }

  useFocusEffect(
    useCallback(() => {
      refreshProfile();
    }, [refreshProfile])
  );

  return (
    <SafeAreaView style={styles.safeArea}>
      <View style={styles.page}>
        <ScrollView
          showsVerticalScrollIndicator={false}
          contentContainerStyle={styles.scrollContent}
        >
          <Text style={styles.header}>
            Perfil
          </Text>

          <View style={styles.profileCard}>
            <View style={styles.avatar}>
              <Ionicons name={user.avatarIcon || 'person'} size={40} color={colors.purplePrimary} />
            </View>

                <View style={styles.profileInfo}>
                  <Text style={styles.name}>{user.name || 'Usuário'}</Text>

              <Text style={styles.email}>
                {user.email}
              </Text>
            </View>

            <TouchableOpacity
              style={styles.editProfileButton}
              onPress={() => navigation.navigate('EditProfile')}
              activeOpacity={0.8}
              accessibilityRole="button"
              accessibilityLabel="Editar perfil"
            >
              <Ionicons name="create-outline" size={20} color={colors.purplePrimary} />
            </TouchableOpacity>
          </View>

          <View style={styles.menuList}>
            {menuItems.map((item) => (
              <MenuItem
                key={item.id}
                icon={item.icon}
                title={item.title}
                subtitle={item.subtitle}
                onPress={() => {
                  if (item.id === 'preferences') {
                    navigation.navigate('EditInterests');
                    return;
                  }

                  if (item.id === 'notifications') {
                    navigation.navigate('Notifications');
                    return;
                  }

                  if (item.id === 'institutional') {
                    navigation.navigate('Institutional');
                    return;
                  }

                  handleMenuPress(item);
                }}
              />
            ))}
          </View>

          <TouchableOpacity
            style={styles.logoutButton}
            onPress={() => navigation.replace('Auth')}
            activeOpacity={0.7}
          >
            <Text style={styles.logoutText}>
              Sair
            </Text>
          </TouchableOpacity>

          <TouchableOpacity
            style={styles.deleteButton}
            onPress={confirmDeleteAccount}
            disabled={deletingAccount}
            activeOpacity={0.7}
          >
            <Text style={styles.deleteText}>
              {deletingAccount ? 'Aguarde...' : 'Excluir minha conta'}
            </Text>
          </TouchableOpacity>

          <Text style={styles.version}>
            Versão 1.0.0
          </Text>
        </ScrollView>
      </View>
    </SafeAreaView>
  );
}

const styles = StyleSheet.create({
  safeArea: {
    flex: 1,
    backgroundColor: colors.whiteSoft,
  },

  page: {
    flex: 1,
    backgroundColor: colors.whiteSoft,
  },

  scrollContent: {
    paddingHorizontal: spacing.md,
    paddingTop: spacing.md,
    paddingBottom: 140,
    gap: spacing.lg,
  },

  header: {
    fontSize: typography.size.xl,
    lineHeight: typography.lineHeight.xl,
    fontWeight: typography.fontWeight.bold,
    color: colors.grayDark,
  },

  profileCard: {
    flexDirection: 'row',
    alignItems: 'center',

    backgroundColor: colors.white,
    borderWidth: 1,
    borderColor: colors.grayLight,
    borderRadius: radius.md,

    padding: spacing.lg,
    gap: spacing.md,

    shadowColor: colors.black,
    shadowOffset: {
      width: 0,
      height: 4,
    },
    shadowOpacity: 0.04,
    shadowRadius: 10,
    elevation: 2,
  },

  avatar: {
    width: 84,
    height: 84,
    borderRadius: 42,
    backgroundColor: colors.purpleSoft,
    alignItems: 'center',
    justifyContent: 'center',
    borderWidth: 1,
    borderColor: colors.purpleLight,
  },

  profileInfo: {
    flex: 1,
    gap: spacing.xs,
  },

  editProfileButton: {
    width: 40,
    height: 40,
    borderRadius: radius.pill,
    backgroundColor: colors.whiteSoft,
    borderWidth: 1,
    borderColor: colors.grayLight,
    alignItems: 'center',
    justifyContent: 'center',
  },

  name: {
    fontSize: typography.size.xl,
    lineHeight: typography.lineHeight.xl,
    fontWeight: typography.fontWeight.bold,
    color: colors.grayDark,
  },

  email: {
    fontSize: typography.size.sm,
    lineHeight: typography.lineHeight.sm,
    color: colors.grayText,
  },

  menuList: {
    gap: spacing.md,
  },

  menuItem: {
    backgroundColor: colors.white,

    borderWidth: 1,
    borderColor: colors.grayLight,
    borderRadius: radius.md,

    paddingHorizontal: spacing.md,
    paddingVertical: spacing.md,

    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'space-between',

    shadowColor: colors.black,
    shadowOpacity: 0.04,
    shadowRadius: 10,
    shadowOffset: {
      width: 0,
      height: 4,
    },

    elevation: 2,
  },

  menuLeft: {
    flex: 1,
    flexDirection: 'row',
    alignItems: 'center',
    gap: spacing.md,
  },

  menuCopy: {
    flex: 1,
  },

  iconContainer: {
    width: 52,
    height: 52,
    borderRadius: 26,

    backgroundColor: colors.purpleSoft,

    alignItems: 'center',
    justifyContent: 'center',
  },

  menuTitle: {
    fontSize: typography.size.md,
    lineHeight: typography.lineHeight.md,
    fontWeight: typography.fontWeight.semiBold,
    color: colors.grayDark,
  },

  menuSubtitle: {
    marginTop: 2,

    fontSize: typography.size.sm,
    lineHeight: typography.lineHeight.sm,
    color: colors.grayText,
  },

  chevronContainer: {
    width: 32,
    height: 32,
    borderRadius: radius.pill,
    backgroundColor: colors.whiteSoft,
    alignItems: 'center',
    justifyContent: 'center',
  },

  logoutButton: {
    alignSelf: 'flex-end',

    paddingHorizontal: spacing.lg,
    paddingVertical: spacing.md,
  },

  logoutText: {
    fontSize: typography.size.md,
    lineHeight: typography.lineHeight.md,
    color: colors.peachBurnt,
    fontWeight: typography.fontWeight.semiBold,
  },

  deleteButton: {
    alignSelf: 'center',
    paddingHorizontal: spacing.lg,
    paddingVertical: spacing.md,
  },

  deleteText: {
    fontSize: typography.size.sm,
    lineHeight: typography.lineHeight.sm,
    color: colors.grayText,
    textDecorationLine: 'underline',
  },

  version: {
    marginTop: spacing.xxl,

    textAlign: 'center',

    fontSize: typography.size.sm,
    lineHeight: typography.lineHeight.sm,
    color: colors.grayText,
  },
});
